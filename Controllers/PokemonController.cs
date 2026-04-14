using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class PokemonController : Controller
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 24)
        {
            var model = await _pokemonService.GetPokemonPageAsync(page, pageSize);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var pokemon = await _pokemonService.GetPokemonDetailsAsync(id);

            if (pokemon == null)
            {
                return NotFound();
            }

            return View(pokemon);
        }
    }
}
