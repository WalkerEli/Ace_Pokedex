using System.Text.RegularExpressions;
using TeamAceProject.Infrastructure;
using TeamAceProject.Models.Dtos.PokeApi;
using TeamAceProject.Models.ViewModels.Pokemon;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly IPokeApiService _pokeApiService;

        public PokemonService(IPokeApiService pokeApiService)
        {
            _pokeApiService = pokeApiService;
        }

        public async Task<PokemonListPageViewModel> GetPokemonPageAsync(int pageNumber, int pageSize)
        {
            int safePageNumber = Math.Max(1, pageNumber);
            int safePageSize = Math.Clamp(pageSize, 1, 60);
            int offset = (safePageNumber - 1) * safePageSize;

            PokemonListResponseDto? response = await _pokeApiService.GetPokemonListAsync(safePageSize, offset);

            PokemonListPageViewModel model = new PokemonListPageViewModel
            {
                PageNumber = safePageNumber,
                PageSize = safePageSize,
                TotalCount = response?.Count ?? 0,
            };

            if (response == null)
            {
                return model;
            }

            foreach (NamedApiResourceDto item in response.Results)
            {
                model.Pokemon.Add(new PokemonListItemViewModel
                {
                    Id = ExtractIdFromUrl(item.Url),
                    Name = item.Name,
                    DisplayName = ToDisplayName(item.Name),
                });
            }

            return model;
        }

        public async Task<PokemonDetailViewModel?> GetPokemonDetailsAsync(string nameOrId)
        {
            PokemonDetailDto? dto = await GetPokemonAsync(nameOrId);

            if (dto == null)
            {
                return null;
            }

            PokemonDetailViewModel model = new PokemonDetailViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                DisplayName = ToDisplayName(dto.Name),
                SpriteUrl = dto.Sprites.Front_Default,
                Abilities = dto.Abilities.Select(ability => ToDisplayName(ability.Ability.Name)).ToList(),
                Types = dto.Types.Select(t => ToDisplayName(t.Type.Name)).ToList(),
                Weaknesses = TypeChart.GetWeaknesses(dto.Types.Select(t => ToDisplayName(t.Type.Name))).ToList(),
                SuperEffectiveAgainst = TypeChart.GetSuperEffectiveAgainst(dto.Types.Select(t => ToDisplayName(t.Type.Name))).ToList(),
                Moves = dto.Moves
                    .Take(24)
                    .Select(move => new MoveSummaryViewModel
                    {
                        Name = move.Move.Name,
                        DisplayName = ToDisplayName(move.Move.Name),
                    })
                    .ToList(),
            };

            foreach (PokemonStatDto stat in dto.Stats)
            {
                model.Stats.Add(new PokemonStatViewModel
                {
                    Name = ToDisplayName(stat.Stat.Name),
                    Value = stat.Base_Stat,
                });
            }

            return model;
        }

        private async Task<PokemonDetailDto?> GetPokemonAsync(string nameOrId)
        {
            if (int.TryParse(nameOrId, out int id))
            {
                return await _pokeApiService.GetPokemonByIdAsync(id);
            }

            return await _pokeApiService.GetPokemonByNameAsync(nameOrId);
        }

        private static int ExtractIdFromUrl(string url)
        {
            Match match = Regex.Match(url, @"/(\d+)/?$");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }

        private static string ToDisplayName(string value)
        {
            string normalized = value.Replace('-', ' ');
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized);
        }
    }
}
