using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TeamAceProject.Models.Dtos.PokeApi;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Services
{
    public class PokeApiService : IPokeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public PokeApiService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public Task<PokemonListResponseDto?> GetPokemonListAsync(int limit, int offset)
        {
            return GetAsync<PokemonListResponseDto>($"pokemon?limit={limit}&offset={offset}");
        }

        public Task<PokemonListResponseDto?> GetMoveListAsync(int limit, int offset)
        {
            return GetAsync<PokemonListResponseDto>($"move?limit={limit}&offset={offset}");
        }

        public Task<PokemonDetailDto?> GetPokemonByNameAsync(string name)
        {
            return GetAsync<PokemonDetailDto>($"pokemon/{NormalizeKey(name)}");
        }

        public Task<PokemonDetailDto?> GetPokemonByIdAsync(int id)
        {
            return GetAsync<PokemonDetailDto>($"pokemon/{id}");
        }

        public Task<AbilityDetailDto?> GetAbilityByNameAsync(string name)
        {
            return GetAsync<AbilityDetailDto>($"ability/{NormalizeKey(name)}");
        }

        public Task<MoveDetailDto?> GetMoveByNameAsync(string name)
        {
            return GetAsync<MoveDetailDto>($"move/{NormalizeKey(name)}");
        }

        public Task<NatureDetailDto?> GetNatureByNameAsync(string name)
        {
            return GetAsync<NatureDetailDto>($"nature/{NormalizeKey(name)}");
        }

        public Task<ItemDetailDto?> GetItemByNameAsync(string name)
        {
            return GetAsync<ItemDetailDto>($"item/{NormalizeKey(name)}");
        }

        public Task<PokemonListResponseDto?> GetAbilityListAsync(int limit, int offset)
        {
            return GetAsync<PokemonListResponseDto>($"ability?limit={limit}&offset={offset}");
        }

        public Task<PokemonListResponseDto?> GetNatureListAsync()
        {
            return GetAsync<PokemonListResponseDto>("nature?limit=100");
        }

        public Task<ItemAttributeDto?> GetItemAttributeAsync(string name)
        {
            return GetAsync<ItemAttributeDto>($"item-attribute/{NormalizeKey(name)}");
        }

        private async Task<T?> GetAsync<T>(string relativeUrl)
        {
            if (_cache.TryGetValue(relativeUrl, out T? cachedResult))
            {
                return cachedResult;
            }

            HttpResponseMessage response = await _httpClient.GetAsync(relativeUrl);

            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            await using Stream stream = await response.Content.ReadAsStreamAsync();
            T? result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);

            if (result != null)
            {
                _cache.Set(relativeUrl, result, TimeSpan.FromMinutes(20));
            }

            return result;
        }

        private static string NormalizeKey(string value)
        {
            return value.Trim().ToLowerInvariant();
        }
    }
}
