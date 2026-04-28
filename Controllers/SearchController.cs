using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Pokemon;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers;

public class SearchController : Controller
{
    private readonly IPokemonService _pokemonService;

    public SearchController(IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return View(new List<PokemonListItemViewModel>());

        List<PokemonListItemViewModel> results = await _pokemonService.SearchPokemonAsync(query);
        ViewData["Query"] = query;
        return View(results);
    }
}
