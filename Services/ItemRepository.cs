using System.Text.RegularExpressions;
using TeamAceProject.Models.Dtos.PokeApi;
using TeamAceProject.Models.ViewModels.Items;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Services
{
    public class ItemRepository : IItemRepository
    {
        private readonly IPokeApiRepository _pokeApiRepo;

        public ItemRepository(IPokeApiRepository PokeApiRepository)
        {
            _pokeApiRepo = PokeApiRepository;
        }

        public async Task<ItemListPageViewModel> GetHeldItemsPageAsync(int pageNumber, int pageSize, string? query = null)
        {
            int safePage = Math.Max(1, pageNumber);
            int safeSize = Math.Clamp(pageSize, 1, 60);

            ItemAttributeDto? attribute = await _pokeApiRepo.GetItemAttributeAsync("holdable");

            ItemListPageViewModel model = new ItemListPageViewModel
            {
                PageNumber = safePage,
                PageSize = safeSize,
                Query = query,
            };

            if (attribute == null)
                return model;

            var filtered = attribute.Items.OrderBy(i => i.Name).AsEnumerable();
            if (!string.IsNullOrWhiteSpace(query))
                filtered = filtered.Where(i => i.Name.Contains(query.Trim().ToLowerInvariant().Replace(' ', '-')));

            model.TotalCount = filtered.Count();

            var page = filtered
                .Skip((safePage - 1) * safeSize)
                .Take(safeSize);

            foreach (NamedApiResourceDto item in page)
            {
                model.Items.Add(new ItemListItemViewModel
                {
                    Name = item.Name,
                    DisplayName = ToDisplayName(item.Name),
                    SpriteUrl = SpriteUrl(item.Name),
                });
            }

            return model;
        }

        public async Task<ItemDetailViewModel?> GetItemDetailsAsync(string name)
        {
            ItemDetailDto? dto = await _pokeApiRepo.GetItemByNameAsync(name);
            if (dto == null)
                return null;

            string displayName = dto.Names
                .FirstOrDefault(n => n.Language.Name == "en")?.Name
                ?? ToDisplayName(dto.Name);

            ItemEffectEntryDto? effectEntry = dto.Effect_Entries
                .FirstOrDefault(e => e.Language.Name == "en");

            return new ItemDetailViewModel
            {
                Name = dto.Name,
                DisplayName = displayName,
                SpriteUrl = dto.Sprites.Default ?? SpriteUrl(dto.Name),
                Category = ToDisplayName(dto.Category.Name),
                Effect = effectEntry?.Effect ?? string.Empty,
                ShortEffect = effectEntry?.Short_Effect ?? string.Empty,
            };
        }

        private static string SpriteUrl(string name)
            => $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/items/{name}.png";

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
