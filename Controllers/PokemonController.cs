using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Pokemon;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    // Fetches and displays Pokemon data sourced live from PokeAPI
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepo;

        public PokemonController(IPokemonRepository PokemonRepository)
        {
            _pokemonRepo = PokemonRepository;
        }

        // Returns a paginated list of Pokemon
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 24, string? query = null)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                var results = await _pokemonRepo.SearchPokemonAsync(query);
                return View(new PokemonListPageViewModel
                {
                    Pokemon = results,
                    Query = query,
                    PageNumber = 1,
                    PageSize = results.Count,
                    TotalCount = results.Count
                });
            }

            var model = await _pokemonRepo.GetPokemonPageAsync(page, pageSize);
            return View(model);
        }

        // Returns the full details page for a single Pokemon by name or ID
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var pokemon = await _pokemonRepo.GetPokemonDetailsAsync(id);

            if (pokemon == null)
            {
                return NotFound();
            }

            return View(pokemon);
        }
    }
}
