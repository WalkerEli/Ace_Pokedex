using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Auth;
using TeamAceProject.Models.ViewModels.Users;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<UserSummaryViewModel>> GetAllUsersAsync()
        {
            return await _context.Users.AsNoTracking()
                .OrderBy(user => user.Username)
                .Select(user => new UserSummaryViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                })
                .ToListAsync();
        }

        public async Task<UserDetailsViewModel?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.AsNoTracking()
                .Where(user => user.Id == userId)
                .Select(user => new UserDetailsViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FavoritePokemonId = user.FavoritePokemonId,
                    FavoritePokemonName = user.FavoritePokemonName,
                    Bio = user.Bio,
                    TeamCount = user.Teams.Count,
                    PostCount = user.Posts.Count,
                    Teams = user.Teams
                        .Select(team => new TeamSummaryViewModel
                        {
                            Id = team.Id,
                            Name = team.Name
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User?> RegisterUserAsync(RegisterInputModel input)
        {
            string normalizedUsername = input.Username.Trim();
            string normalizedEmail = input.Email.Trim().ToLowerInvariant();

            bool usernameTaken = await _context.Users.AnyAsync(user => user.Username.ToLower() == normalizedUsername.ToLower());
            if (usernameTaken)
            {
                return null;
            }

            bool emailTaken = await _context.Users.AnyAsync(user => user.Email.ToLower() == normalizedEmail);
            if (emailTaken)
            {
                return null;
            }

            User user = new User
            {
                Id = Guid.NewGuid(),
                Username = normalizedUsername,
                Email = normalizedEmail,
                CreatedAt = DateTime.UtcNow,
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, input.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> AuthenticateUserAsync(string usernameOrEmail, string password)
        {
            string normalizedInput = usernameOrEmail.Trim().ToLowerInvariant();

            User? user = await _context.Users.FirstOrDefaultAsync(user =>
                user.Username.ToLower() == normalizedInput || user.Email.ToLower() == normalizedInput);

            if (user == null)
            {
                return null;
            }

            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, password);
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public async Task<bool> SetFavoritePokemonAsync(Guid userId, int pokemonId, string pokemonName)
        {
            User? user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.FavoritePokemonId = pokemonId;
            user.FavoritePokemonName = pokemonName.Trim().ToLowerInvariant();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetBioAsync(Guid userId, string bio)
        {
            User? user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Bio = bio.Trim();
            await _context.SaveChangesAsync();
            return true;
        }
    }
}