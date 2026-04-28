namespace TeamAceProject.Models.ViewModels.Abilities
{
    public class AbilityListPageViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string? Query { get; set; }
        public List<AbilityListItemViewModel> Abilities { get; set; } = new();
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber * PageSize < TotalCount;
    }

    public class AbilityListItemViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
