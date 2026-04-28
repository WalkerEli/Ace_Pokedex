namespace TeamAceProject.Models.ViewModels.Moves
{
    public class MoveListPageViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string? Query { get; set; }
        public List<MoveListItemViewModel> Moves { get; set; } = new();
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber * PageSize < TotalCount;
    }

    public class MoveListItemViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string DamageClass { get; set; } = string.Empty;
    }
}
