using TeamAceProject.Models.Dtos.PokeApi;

namespace TeamAceProject.Services.Interfaces
{
    public interface IPokeApiService
    {
        Task<PokemonListResponseDto?> GetPokemonListAsync(int limit, int offset);
        Task<PokemonListResponseDto?> GetMoveListAsync(int limit, int offset);
        Task<PokemonListResponseDto?> GetAbilityListAsync(int limit, int offset);
        Task<PokemonDetailDto?> GetPokemonByNameAsync(string name);
        Task<PokemonDetailDto?> GetPokemonByIdAsync(int id);
        Task<AbilityDetailDto?> GetAbilityByNameAsync(string name);
        Task<MoveDetailDto?> GetMoveByNameAsync(string name);
        Task<NatureDetailDto?> GetNatureByNameAsync(string name);
        Task<ItemDetailDto?> GetItemByNameAsync(string name);
        Task<PokemonListResponseDto?> GetNatureListAsync();
        Task<ItemAttributeDto?> GetItemAttributeAsync(string name);
    }
}
