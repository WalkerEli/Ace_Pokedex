using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Infrastructure;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    // Handles user profile viewing and profile updates
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository DbUserRepository)
        {
            _userRepo = DbUserRepository;
        }

        // Shows the public profile page for any user by their ID
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Convenience action that redirects the logged-in user to their own profile
        [Authorize]
        [HttpGet]
        public IActionResult Me()
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction(nameof(Details), new { id = currentUserId.Value });
        }

        // Saves the current user's bio text after verifying ownership
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetBio(Guid userId, string bio)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != userId)
                return Forbid();

            await _userRepo.SetBioAsync(userId, bio ?? string.Empty);
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        // Updates the current user's favorite Pokemon after verifying ownership
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FavoritePokemon(Guid userId, int pokemonId, string pokemonName)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue || currentUserId.Value != userId)
            {
                return Forbid();
            }

            bool updated = await _userRepo.SetFavoritePokemonAsync(userId, pokemonId, pokemonName);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id = userId });
        }
    }
}
