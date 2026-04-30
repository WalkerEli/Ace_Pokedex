namespace TeamAceProject.Models.ViewModels.Items
{
    public class ItemDetailViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? SpriteUrl { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Effect { get; set; } = string.Empty;
        public string ShortEffect { get; set; } = string.Empty;
    }
}
