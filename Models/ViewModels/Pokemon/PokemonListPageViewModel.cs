namespace TeamAceProject.Models.ViewModels.Pokemon
{
    public class PokemonListPageViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string? Query { get; set; }
        public List<PokemonListItemViewModel> Pokemon { get; set; } = new List<PokemonListItemViewModel>();
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber * PageSize < TotalCount;
    }
}
