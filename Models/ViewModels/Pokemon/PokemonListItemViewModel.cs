namespace TeamAceProject.Models.ViewModels.Pokemon
{
    public class PokemonListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string DetailsKey => Name;
    }
}
