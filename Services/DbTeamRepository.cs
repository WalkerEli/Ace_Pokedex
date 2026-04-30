using Microsoft.EntityFrameworkCore;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Teams;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Services
{
    public class DbTeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPokeApiRepository _pokeApiRepo;

        public DbTeamRepository(ApplicationDbContext context, IPokeApiRepository PokeApiRepository)
        {
            _context = context;
            _pokeApiRepo = PokeApiRepository;
        }

        public async Task<List<TeamListItemViewModel>> GetTeamsByUserAsync(int userId)
        {
            return await _context.Teams.AsNoTracking()
                .Where(team => team.UserId == userId)
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

        public async Task<TeamDetailsViewModel?> GetTeamByIdAsync(int teamId)
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
                    Types = string.IsNullOrWhiteSpace(member.PokemonTypes)
                        ? new List<string>()
                        : member.PokemonTypes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
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

        public async Task<List<TeamOptionViewModel>> GetUserTeamOptionsAsync(int userId)
        {
            return await _context.Teams
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.Name)
                .Select(t => new TeamOptionViewModel { Id = t.Id, Name = t.Name })
                .ToListAsync();
        }

        public async Task<int?> GetTeamOwnerIdAsync(int teamId)
        {
            return await _context.Teams
                .Where(t => t.Id == teamId)
                .Select(t => (int?)t.UserId)
                .FirstOrDefaultAsync();
        }

        public async Task<Team> CreateTeamAsync(Team team)
        {
            team.CreatedAt = DateTime.UtcNow;
            team.UpdatedAt = DateTime.UtcNow;
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<bool> RenameTeamAsync(int teamId, string newName)
        {
            Team? team = await _context.Teams.FindAsync(teamId);
            if (team == null) return false;
            team.Name = newName.Trim();
            team.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTeamAsync(int teamId)
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

        public async Task<TeamMember?> GetTeamMemberByIdAsync(int teamMemberId)
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
            var pokemon = await _pokeApiRepo.GetPokemonByNameAsync(normalizedPokemonName);
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

            var usedSlots = await _context.TeamMembers
                .Where(m => m.TeamId == input.TeamId)
                .Select(m => m.SlotIndex)
                .ToListAsync();

            int nextSlot = Enumerable.Range(1, 6).FirstOrDefault(s => !usedSlots.Contains(s));
            if (nextSlot == 0)
                throw new InvalidOperationException("This team is full (6 Pokémon maximum).");

            TeamMember teamMember = new TeamMember
            {
                TeamId = input.TeamId,
                SlotIndex = nextSlot,
                PokemonId = pokemon.Id,
                PokemonName = pokemon.Name,
                PokemonSpriteUrl = pokemon.Sprites.Front_Default,
                PokemonTypes = string.Join(",", pokemon.Types.Select(t => ToDisplayName(t.Type.Name))),
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

        public async Task<TeamMember> EditTeamMemberAsync(EditTeamMemberInputModel input)
        {
            TeamMember? member = await _context.TeamMembers
                .FirstOrDefaultAsync(m => m.Id == input.TeamMemberId);

            if (member == null)
                throw new InvalidOperationException("Team member not found.");

            string normalizedPokemonName = NormalizeApiName(input.PokemonName);
            var pokemon = await _pokeApiRepo.GetPokemonByNameAsync(normalizedPokemonName);
            if (pokemon == null)
                throw new InvalidOperationException("Pokemon not found in PokeAPI.");

            if (!string.IsNullOrWhiteSpace(input.AbilityName))
            {
                string normalizedAbility = NormalizeApiName(input.AbilityName);
                bool matchingAbility = pokemon.Abilities.Any(a => a.Ability.Name == normalizedAbility);
                if (!matchingAbility)
                    throw new InvalidOperationException("That ability is not available for the selected Pokemon.");
            }

            member.PokemonId = pokemon.Id;
            member.PokemonName = pokemon.Name;
            member.PokemonSpriteUrl = pokemon.Sprites.Front_Default;
            member.PokemonTypes = string.Join(",", pokemon.Types.Select(t => ToDisplayName(t.Type.Name)));
            member.AbilityName = NormalizeOptional(input.AbilityName);
            member.NatureName = NormalizeOptional(input.NatureName);
            member.HeldItemName = NormalizeOptional(input.HeldItemName);

            var existingMoves = await _context.TeamMemberMoves
                .Where(m => m.TeamMemberId == input.TeamMemberId)
                .ToListAsync();
            _context.TeamMemberMoves.RemoveRange(existingMoves);

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
                bool matchingMove = pokemon.Moves.Any(m => m.Move.Name == normalizedMove);
                if (!matchingMove)
                    throw new InvalidOperationException($"{moveName} is not a valid move for {pokemon.Name}.");

                _context.TeamMemberMoves.Add(new TeamMemberMove
                {
                    TeamMemberId = member.Id,
                    MoveSlot = moveSlot,
                    MoveName = normalizedMove,
                });

                moveSlot++;
            }

            Team? team = await _context.Teams.FindAsync(member.TeamId);
            if (team != null) team.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<bool> RemoveTeamMemberAsync(int teamMemberId)
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
            teamMemberMove.MoveName = NormalizeApiName(teamMemberMove.MoveName);
            _context.TeamMemberMoves.Add(teamMemberMove);
            await _context.SaveChangesAsync();
            return teamMemberMove;
        }

        public async Task<bool> RemoveTeamMemberMoveAsync(int teamMemberMoveId)
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
