using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Pokemon;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers;

public class SearchController : Controller
{
    private readonly IPokemonRepository _pokemonRepo;

    public SearchController(IPokemonRepository PokemonRepository)
    {
        _pokemonRepo = PokemonRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return View(new List<PokemonListItemViewModel>());

        List<PokemonListItemViewModel> results = await _pokemonRepo.SearchPokemonAsync(query);
        ViewData["Query"] = query;
        return View(results);
    }
}
