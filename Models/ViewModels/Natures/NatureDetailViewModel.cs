namespace TeamAceProject.Models.ViewModels.Natures
{
    public class NatureDetailViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? IncreasedStat { get; set; }
        public string? DecreasedStat { get; set; }
        public string? LikesFlavor { get; set; }
        public string? HatesFlavor { get; set; }
        public bool IsNeutral => IncreasedStat == null && DecreasedStat == null;
    }
}
