using TeamAceProject.Models.ViewModels.Pokemon;

namespace TeamAceProject.Services.Interfaces
{
    public interface IPokemonRepository
    {
        Task<PokemonListPageViewModel> GetPokemonPageAsync(int pageNumber, int pageSize);
        Task<PokemonDetailViewModel?> GetPokemonDetailsAsync(string nameOrId);
        Task<List<PokemonListItemViewModel>> SearchPokemonAsync(string query);
    }
}
