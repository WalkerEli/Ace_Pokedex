using TeamAceProject.Infrastructure;
using TeamAceProject.Models.Dtos.PokeApi;
using TeamAceProject.Models.ViewModels.Moves;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Services
{
    public class MovesService : IMovesService
    {
        private readonly IPokeApiService _pokeApiService;

        public MovesService(IPokeApiService pokeApiService)
        {
            _pokeApiService = pokeApiService;
        }

        public async Task<MoveListPageViewModel> GetMovesPageAsync(int pageNumber, int pageSize, string? query = null)
        {
            int safePage = Math.Max(1, pageNumber);
            int safeSize = Math.Clamp(pageSize, 1, 40);

            // Fetch a large batch when searching so we can filter, otherwise fetch one page.
            bool isSearch = !string.IsNullOrWhiteSpace(query);
            int fetchLimit = isSearch ? 2000 : safeSize;
            int fetchOffset = isSearch ? 0 : (safePage - 1) * safeSize;

            PokemonListResponseDto? list = await _pokeApiService.GetMoveListAsync(fetchLimit, fetchOffset);

            MoveListPageViewModel model = new MoveListPageViewModel
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
                results = results.Where(m => m.Name.Contains(normalized));
            }

            var paged = results.Skip(isSearch ? (safePage - 1) * safeSize : 0).Take(safeSize).ToList();
            model.TotalCount = isSearch ? results.Count() : list.Count;

            // Fetch details for this page in parallel to get type info.
            MoveDetailDto?[] details = await Task.WhenAll(
                paged.Select(m => _pokeApiService.GetMoveByNameAsync(m.Name)));

            foreach (MoveDetailDto? dto in details)
            {
                if (dto == null) continue;
                model.Moves.Add(new MoveListItemViewModel
                {
                    Name = dto.Name,
                    DisplayName = ToDisplayName(dto.Name),
                    Type = ToDisplayName(dto.Type.Name),
                    DamageClass = ToDisplayName(dto.Damage_Class.Name),
                });
            }

            return model;
        }

        public async Task<MoveDetailViewModel?> GetMoveDetailsAsync(string name)
        {
            MoveDetailDto? dto = await _pokeApiService.GetMoveByNameAsync(name);
            if (dto == null)
                return null;

            string displayName = dto.Names
                .FirstOrDefault(n => n.Language.Name == "en")?.Name
                ?? ToDisplayName(dto.Name);

            MoveEffectEntryDto? effectEntry = dto.Effect_Entries
                .FirstOrDefault(e => e.Language.Name == "en");

            string moveType = ToDisplayName(dto.Type.Name);
            var (superEffective, notVery, noEffect) = TypeChart.GetMoveMatchups(moveType);

            return new MoveDetailViewModel
            {
                Name = dto.Name,
                DisplayName = displayName,
                Type = moveType,
                DamageClass = ToDisplayName(dto.Damage_Class.Name),
                Power = dto.Power,
                Accuracy = dto.Accuracy,
                PP = dto.PP,
                ShortEffect = effectEntry?.Short_Effect ?? string.Empty,
                Effect = effectEntry?.Effect ?? string.Empty,
                SuperEffectiveAgainst = superEffective,
                NotVeryEffectiveAgainst = notVery,
                NoEffectAgainst = noEffect,
            };
        }

        private static string ToDisplayName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            string normalized = value.Replace('-', ' ');
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized);
        }
    }
}
