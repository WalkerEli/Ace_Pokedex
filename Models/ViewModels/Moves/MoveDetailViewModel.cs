namespace TeamAceProject.Models.ViewModels.Moves
{
    public class MoveDetailViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string DamageClass { get; set; } = string.Empty;
        public int? Power { get; set; }
        public int? Accuracy { get; set; }
        public int PP { get; set; }
        public string ShortEffect { get; set; } = string.Empty;
        public string Effect { get; set; } = string.Empty;
        public List<string> SuperEffectiveAgainst { get; set; } = new();
        public List<string> NotVeryEffectiveAgainst { get; set; } = new();
        public List<string> NoEffectAgainst { get; set; } = new();
    }
}
