namespace TeamAceProject.Models.Dtos.PokeApi
{
    public class PokemonListResponseDto
    {
        public int Count { get; set; }
        public string? Next { get; set; }
        public string? Previous { get; set; }
        public List<NamedApiResourceDto> Results { get; set; } = new List<NamedApiResourceDto>();
    }
}
