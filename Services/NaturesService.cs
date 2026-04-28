using TeamAceProject.Models.Dtos.PokeApi;
using TeamAceProject.Models.ViewModels.Natures;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Services
{
    public class NaturesService : INaturesService
    {
        private readonly IPokeApiService _pokeApiService;

        public NaturesService(IPokeApiService pokeApiService)
        {
            _pokeApiService = pokeApiService;
        }

        public async Task<NatureListViewModel> GetAllNaturesAsync(string? query = null)
        {
            PokemonListResponseDto? list = await _pokeApiService.GetNatureListAsync();

            NatureListViewModel model = new NatureListViewModel { Query = query };

            if (list == null)
                return model;

            IEnumerable<NamedApiResourceDto> results = list.Results.OrderBy(n => n.Name);

            if (!string.IsNullOrWhiteSpace(query))
            {
                string normalized = query.Trim().ToLowerInvariant().Replace(' ', '-');
                results = results.Where(n => n.Name.Contains(normalized));
            }

            // Fetch all nature details in parallel (only 25 total, heavily cached).
            NatureDetailDto?[] details = await Task.WhenAll(
                results.Select(n => _pokeApiService.GetNatureByNameAsync(n.Name)));

            foreach (NatureDetailDto? dto in details)
            {
                if (dto == null) continue;
                model.Natures.Add(new NatureListItemViewModel
                {
                    Name = dto.Name,
                    DisplayName = ToDisplayName(dto.Name),
                    IncreasedStat = dto.Increased_Stat != null ? ToDisplayName(dto.Increased_Stat.Name) : null,
                    DecreasedStat = dto.Decreased_Stat != null ? ToDisplayName(dto.Decreased_Stat.Name) : null,
                });
            }

            return model;
        }

        public async Task<NatureDetailViewModel?> GetNatureDetailsAsync(string name)
        {
            NatureDetailDto? dto = await _pokeApiService.GetNatureByNameAsync(name);
            if (dto == null)
                return null;

            string displayName = dto.Names
                .FirstOrDefault(n => n.Language.Name == "en")?.Name
                ?? ToDisplayName(dto.Name);

            return new NatureDetailViewModel
            {
                Name = dto.Name,
                DisplayName = displayName,
                IncreasedStat = dto.Increased_Stat != null ? ToDisplayName(dto.Increased_Stat.Name) : null,
                DecreasedStat = dto.Decreased_Stat != null ? ToDisplayName(dto.Decreased_Stat.Name) : null,
                LikesFlavor = dto.Likes_Flavor != null ? ToDisplayName(dto.Likes_Flavor.Name) : null,
                HatesFlavor = dto.Hates_Flavor != null ? ToDisplayName(dto.Hates_Flavor.Name) : null,
            };
        }

        private static string ToDisplayName(string value)
        {
            string normalized = value.Replace('-', ' ');
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized);
        }
    }
}
