namespace TeamAceProject.Models.ViewModels.Abilities
{
    public class AbilityDetailViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string ShortEffect { get; set; } = string.Empty;
        public string Effect { get; set; } = string.Empty;
        public List<AbilityPokemonViewModel> Pokemon { get; set; } = new();
    }

    public class AbilityPokemonViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
        public string SpriteUrl => $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{Id}.png";
    }
}
