namespace TeamAceProject.Models.ViewModels.Items
{
    public class ItemListPageViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string? Query { get; set; }
        public List<ItemListItemViewModel> Items { get; set; } = new();
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber * PageSize < TotalCount;
    }

    public class ItemListItemViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? SpriteUrl { get; set; }
    }
}
