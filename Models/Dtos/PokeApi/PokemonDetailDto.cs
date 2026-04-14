namespace TeamAceProject.Models.Dtos.PokeApi
{
    public class PokemonDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PokemonSpritesDto Sprites { get; set; } = new PokemonSpritesDto();
        public List<PokemonStatDto> Stats { get; set; } = new List<PokemonStatDto>();
        public List<PokemonAbilityEntryDto> Abilities { get; set; } = new List<PokemonAbilityEntryDto>();
        public List<PokemonMoveEntryDto> Moves { get; set; } = new List<PokemonMoveEntryDto>();
    }

    public class PokemonSpritesDto
    {
        public string? Front_Default { get; set; }
    }

    public class PokemonStatDto
    {
        public int Base_Stat { get; set; }
        public NamedApiResourceDto Stat { get; set; } = new NamedApiResourceDto();
    }

    public class PokemonAbilityEntryDto
    {
        public NamedApiResourceDto Ability { get; set; } = new NamedApiResourceDto();
    }

    public class PokemonMoveEntryDto
    {
        public NamedApiResourceDto Move { get; set; } = new NamedApiResourceDto();
    }
}
