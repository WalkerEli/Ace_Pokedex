namespace TeamAceProject.Models.ViewModels.Natures
{
    public class NatureListViewModel
    {
        public List<NatureListItemViewModel> Natures { get; set; } = new();
        public string? Query { get; set; }
    }

    public class NatureListItemViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? IncreasedStat { get; set; }
        public string? DecreasedStat { get; set; }
        public bool IsNeutral => IncreasedStat == null && DecreasedStat == null;
    }
}
