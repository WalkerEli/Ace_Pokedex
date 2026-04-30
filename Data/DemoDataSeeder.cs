using Microsoft.AspNetCore.Identity;
using TeamAceProject.Models.Entities;
using TeamAceProject.Services;

namespace TeamAceProject.Data
{
    public static class DemoDataSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            User user = new User
            {
                Id = 1,
                Username = "demo-trainer",
                Email = "demo@teamace.local",
                FavoritePokemonId = 25,
                FavoritePokemonName = "pikachu",
            };

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, "demo123!");

            Team team = new Team
            {
                Id = 2,
                UserId = user.Id,
                Name = "sample squad",
                IsPublic = true,
                UpdatedAt = DateTime.UtcNow,
            };

            TeamMember member = new TeamMember
            {
                Id = 3,
                TeamId = team.Id,
                SlotIndex = 1,
                PokemonId = 25,
                PokemonName = "pikachu",
                AbilityName = "static",
                NatureName = "jolly",
                HeldItemName = "light-ball",
                PokemonSpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/25.png",
            };

            TeamMemberMove move1 = new TeamMemberMove
            {
                TeamMemberId = member.Id,
                MoveSlot = 1,
                MoveName = "thunderbolt",
            };

            TeamMemberMove move2 = new TeamMemberMove
            {
                TeamMemberId = member.Id,
                MoveSlot = 2,
                MoveName = "volt-tackle",
            };

            Post post = new Post
            {
                Id = 4,
                TeamId = team.Id,
                UserId = user.Id,
                Caption = "starting point for the rebuilt api-first project",
            };

            context.Users.Add(user);
            context.Teams.Add(team);
            context.TeamMembers.Add(member);
            context.TeamMemberMoves.AddRange(move1, move2);
            context.Posts.Add(post);
            context.SaveChanges();
        }
    }
}
