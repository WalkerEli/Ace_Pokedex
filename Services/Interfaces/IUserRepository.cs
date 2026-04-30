using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Auth;
using TeamAceProject.Models.ViewModels.Users;

namespace TeamAceProject.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserSummaryViewModel>> GetAllUsersAsync();
        Task<UserDetailsViewModel?> GetUserByIdAsync(int userId);
        Task<User?> RegisterUserAsync(RegisterInputModel input);
        Task<User?> AuthenticateUserAsync(string usernameOrEmail, string password);
        Task<bool> SetFavoritePokemonAsync(int userId, int pokemonId, string pokemonName);
        Task<bool> SetBioAsync(int userId, string bio);
    }
}
