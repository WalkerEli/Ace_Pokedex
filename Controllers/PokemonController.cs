using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    // Fetches and displays Pokemon data sourced live from PokeAPI
    public class PokemonController : Controller
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        // Returns a paginated list of Pokemon
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 24)
        {
            var model = await _pokemonService.GetPokemonPageAsync(page, pageSize);
            return View(model);
        }

        // Returns the full details page for a single Pokemon by name or ID
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
