using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Auth;
using TeamAceProject.Models.ViewModels.Users;

namespace TeamAceProject.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserSummaryViewModel>> GetAllUsersAsync();
        Task<UserDetailsViewModel?> GetUserByIdAsync(Guid userId);
        Task<User?> RegisterUserAsync(RegisterInputModel input);
        Task<User?> AuthenticateUserAsync(string usernameOrEmail, string password);
        Task<bool> SetFavoritePokemonAsync(Guid userId, int pokemonId, string pokemonName);
        Task<bool> SetBioAsync(Guid userId, string bio);
    }
}
