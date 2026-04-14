namespace TeamAceProject.Models.ViewModels.Pokemon
{
    public class PokemonDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? SpriteUrl { get; set; }
        public List<PokemonStatViewModel> Stats { get; set; } = new List<PokemonStatViewModel>();
        public List<string> Abilities { get; set; } = new List<string>();
        public List<MoveSummaryViewModel> Moves { get; set; } = new List<MoveSummaryViewModel>();
    }

    public class PokemonStatViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class MoveSummaryViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
