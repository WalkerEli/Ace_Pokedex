using Microsoft.EntityFrameworkCore;
using TeamAceProject.Data;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Teams;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPokeApiService _pokeApiService;

        public TeamService(ApplicationDbContext context, IPokeApiService pokeApiService)
        {
            _context = context;
            _pokeApiService = pokeApiService;
        }

        public async Task<List<TeamListItemViewModel>> GetAllTeamsAsync()
        {
            return await _context.Teams.AsNoTracking()
                .Include(team => team.User)
                .Include(team => team.TeamMembers)
                .OrderByDescending(team => team.UpdatedAt)
                .Select(team => new TeamListItemViewModel
                {
                    Id = team.Id,
                    Name = team.Name,
                    Username = team.User != null ? team.User.Username : "unknown",
                    IsPublic = team.IsPublic,
                    MemberCount = team.TeamMembers.Count,
                    UpdatedAt = team.UpdatedAt,
                })
                .ToListAsync();
        }

        public async Task<TeamDetailsViewModel?> GetTeamByIdAsync(Guid teamId)
        {
            Team? team = await _context.Teams.AsNoTracking()
                .Include(team => team.User)
                .Include(team => team.TeamMembers)
                    .ThenInclude(member => member.TeamMemberMoves)
                .FirstOrDefaultAsync(team => team.Id == teamId);

            if (team == null)
            {
                return null;
            }

            TeamDetailsViewModel model = new TeamDetailsViewModel
            {
                Id = team.Id,
                UserId = team.UserId,
                Name = team.Name,
                Username = team.User?.Username ?? "unknown",
                IsPublic = team.IsPublic,
                UpdatedAt = team.UpdatedAt,
                NewMember = new AddTeamMemberInputModel { TeamId = team.Id },
            };

            foreach (TeamMember member in team.TeamMembers.OrderBy(member => member.SlotIndex))
            {
                model.Members.Add(new TeamMemberViewModel
                {
                    Id = member.Id,
                    SlotIndex = member.SlotIndex,
                    PokemonId = member.PokemonId,
                    PokemonName = ToDisplayName(member.PokemonName),
                    PokemonSpriteUrl = member.PokemonSpriteUrl,
                    AbilityName = ToDisplayNameOrEmpty(member.AbilityName),
                    NatureName = ToDisplayNameOrEmpty(member.NatureName),
                    HeldItemName = ToDisplayNameOrEmpty(member.HeldItemName),
                    Moves = member.TeamMemberMoves
                        .OrderBy(move => move.MoveSlot)
                        .Select(move => ToDisplayName(move.MoveName))
                        .ToList(),
                });
            }

            return model;
        }

        public async Task<Team> CreateTeamAsync(Team team)
        {
            team.Id = Guid.NewGuid();
            team.CreatedAt = DateTime.UtcNow;
            team.UpdatedAt = DateTime.UtcNow;
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<bool> DeleteTeamAsync(Guid teamId)
        {
            Team? team = await _context.Teams.FindAsync(teamId);

            if (team == null)
            {
                return false;
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TeamMember?> GetTeamMemberByIdAsync(Guid teamMemberId)
        {
            return await _context.TeamMembers
                .Include(member => member.TeamMemberMoves)
                .FirstOrDefaultAsync(member => member.Id == teamMemberId);
        }

        public async Task<TeamMember> AddTeamMemberAsync(AddTeamMemberInputModel input)
        {
            Team? team = await _context.Teams.FindAsync(input.TeamId);
            if (team == null)
            {
                throw new InvalidOperationException("Team not found.");
            }

            string normalizedPokemonName = NormalizeApiName(input.PokemonName);
            var pokemon = await _pokeApiService.GetPokemonByNameAsync(normalizedPokemonName);
            if (pokemon == null)
            {
                throw new InvalidOperationException("Pokemon not found in PokeAPI.");
            }

            if (!string.IsNullOrWhiteSpace(input.AbilityName))
            {
                string normalizedAbility = NormalizeApiName(input.AbilityName);
                bool matchingAbility = pokemon.Abilities.Any(ability => ability.Ability.Name == normalizedAbility);
                if (!matchingAbility)
                {
                    throw new InvalidOperationException("That ability is not available for the selected Pokemon.");
                }
            }

            TeamMember teamMember = new TeamMember
            {
                Id = Guid.NewGuid(),
                TeamId = input.TeamId,
                SlotIndex = input.SlotIndex,
                PokemonId = pokemon.Id,
                PokemonName = pokemon.Name,
                PokemonSpriteUrl = pokemon.Sprites.Front_Default,
                AbilityName = NormalizeOptional(input.AbilityName),
                NatureName = NormalizeOptional(input.NatureName),
                HeldItemName = NormalizeOptional(input.HeldItemName),
            };

            List<string?> moveNames = new List<string?> { input.Move1, input.Move2, input.Move3, input.Move4 };
            int moveSlot = 1;
            foreach (string? moveName in moveNames)
            {
                if (string.IsNullOrWhiteSpace(moveName))
                {
                    moveSlot++;
                    continue;
                }

                string normalizedMove = NormalizeApiName(moveName);
                bool matchingMove = pokemon.Moves.Any(move => move.Move.Name == normalizedMove);
                if (!matchingMove)
                {
                    throw new InvalidOperationException($"{moveName} is not a valid move for {pokemon.Name} in the current PokeAPI data.");
                }

                teamMember.TeamMemberMoves.Add(new TeamMemberMove
                {
                    Id = Guid.NewGuid(),
                    TeamMemberId = teamMember.Id,
                    MoveSlot = moveSlot,
                    MoveName = normalizedMove,
                });

                moveSlot++;
            }

            _context.TeamMembers.Add(teamMember);
            team.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return teamMember;
        }

        public async Task<bool> RemoveTeamMemberAsync(Guid teamMemberId)
        {
            TeamMember? teamMember = await _context.TeamMembers.FindAsync(teamMemberId);
            if (teamMember == null)
            {
                return false;
            }

            Team? team = await _context.Teams.FindAsync(teamMember.TeamId);
            _context.TeamMembers.Remove(teamMember);

            if (team != null)
            {
                team.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TeamMemberMove> AddTeamMemberMoveAsync(TeamMemberMove teamMemberMove)
        {
            teamMemberMove.Id = Guid.NewGuid();
            teamMemberMove.MoveName = NormalizeApiName(teamMemberMove.MoveName);
            _context.TeamMemberMoves.Add(teamMemberMove);
            await _context.SaveChangesAsync();
            return teamMemberMove;
        }

        public async Task<bool> RemoveTeamMemberMoveAsync(Guid teamMemberMoveId)
        {
            TeamMemberMove? move = await _context.TeamMemberMoves.FindAsync(teamMemberMoveId);
            if (move == null)
            {
                return false;
            }

            _context.TeamMemberMoves.Remove(move);
            await _context.SaveChangesAsync();
            return true;
        }

        private static string NormalizeApiName(string value)
        {
            return value.Trim().ToLowerInvariant().Replace(' ', '-');
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : NormalizeApiName(value);
        }

        private static string ToDisplayName(string value)
        {
            string normalized = value.Replace('-', ' ');
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized);
        }

        private static string ToDisplayNameOrEmpty(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : ToDisplayName(value);
        }
    }
}
