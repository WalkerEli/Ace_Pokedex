using System.Text.RegularExpressions;
using TeamAceProject.Models.Dtos.PokeApi;
using TeamAceProject.Models.ViewModels.Abilities;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Services
{
    public class AbilitiesService : IAbilitiesService
    {
        private readonly IPokeApiService _pokeApiService;

        public AbilitiesService(IPokeApiService pokeApiService)
        {
            _pokeApiService = pokeApiService;
        }

        public async Task<AbilityListPageViewModel> GetAbilitiesPageAsync(int pageNumber, int pageSize, string? query = null)
        {
            int safePage = Math.Max(1, pageNumber);
            int safeSize = Math.Clamp(pageSize, 1, 60);

            bool isSearch = !string.IsNullOrWhiteSpace(query);
            int fetchLimit = isSearch ? 2000 : safeSize;
            int fetchOffset = isSearch ? 0 : (safePage - 1) * safeSize;

            PokemonListResponseDto? list = await _pokeApiService.GetAbilityListAsync(fetchLimit, fetchOffset);

            AbilityListPageViewModel model = new AbilityListPageViewModel
            {
                PageNumber = safePage,
                PageSize = safeSize,
                Query = query,
            };

            if (list == null)
                return model;

            IEnumerable<NamedApiResourceDto> results = list.Results;

            if (isSearch)
            {
                string normalized = query!.Trim().ToLowerInvariant().Replace(' ', '-');
                results = results.Where(a => a.Name.Contains(normalized));
            }

            model.TotalCount = isSearch ? results.Count() : list.Count;

            foreach (NamedApiResourceDto item in results.Skip(isSearch ? (safePage - 1) * safeSize : 0).Take(safeSize))
            {
                model.Abilities.Add(new AbilityListItemViewModel
                {
                    Name = item.Name,
                    DisplayName = ToDisplayName(item.Name),
                });
            }

            return model;
        }

        public async Task<AbilityDetailViewModel?> GetAbilityDetailsAsync(string name)
        {
            AbilityDetailDto? dto = await _pokeApiService.GetAbilityByNameAsync(name);
            if (dto == null)
                return null;

            string displayName = dto.Names
                .FirstOrDefault(n => n.Language.Name == "en")?.Name
                ?? ToDisplayName(dto.Name);

            AbilityEffectEntryDto? effectEntry = dto.Effect_Entries
                .FirstOrDefault(e => e.Language.Name == "en");

            var pokemon = dto.Pokemon
                .Select(p => new AbilityPokemonViewModel
                {
                    Id = ExtractIdFromUrl(p.Pokemon.Url),
                    Name = p.Pokemon.Name,
                    DisplayName = ToDisplayName(p.Pokemon.Name),
                    IsHidden = p.Is_Hidden,
                })
                .Where(p => p.Id > 0)
                .OrderBy(p => p.Id)
                .ToList();

            return new AbilityDetailViewModel
            {
                Name = dto.Name,
                DisplayName = displayName,
                ShortEffect = effectEntry?.Short_Effect ?? string.Empty,
                Effect = effectEntry?.Effect ?? string.Empty,
                Pokemon = pokemon,
            };
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
