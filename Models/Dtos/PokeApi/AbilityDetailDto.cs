namespace TeamAceProject.Models.Dtos.PokeApi
{
    public class AbilityDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<AbilityEffectEntryDto> Effect_Entries { get; set; } = new List<AbilityEffectEntryDto>();
    }

    public class AbilityEffectEntryDto
    {
        public string Effect { get; set; } = string.Empty;
        public NamedApiResourceDto Language { get; set; } = new NamedApiResourceDto();
    }
}
