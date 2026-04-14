using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Infrastructure;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

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

            bool updated = await _userService.SetFavoritePokemonAsync(userId, pokemonId, pokemonName);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id = userId });
        }
    }
}
