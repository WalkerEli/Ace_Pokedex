namespace TeamAceProject.Models.Dtos.PokeApi
{
    public class AbilityDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<AbilityNameDto> Names { get; set; } = new();
        public List<AbilityEffectEntryDto> Effect_Entries { get; set; } = new();
        public List<AbilityPokemonEntryDto> Pokemon { get; set; } = new();
    }

    public class AbilityNameDto
    {
        public string Name { get; set; } = string.Empty;
        public NamedApiResourceDto Language { get; set; } = new NamedApiResourceDto();
    }

    public class AbilityEffectEntryDto
    {
        public string Effect { get; set; } = string.Empty;
        public string Short_Effect { get; set; } = string.Empty;
        public NamedApiResourceDto Language { get; set; } = new NamedApiResourceDto();
    }

    public class AbilityPokemonEntryDto
    {
        public bool Is_Hidden { get; set; }
        public NamedApiResourceDto Pokemon { get; set; } = new NamedApiResourceDto();
    }
}
