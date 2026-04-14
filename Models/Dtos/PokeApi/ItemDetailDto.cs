namespace TeamAceProject.Models.Dtos.PokeApi
{
    public class ItemDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ItemEffectEntryDto> Effect_Entries { get; set; } = new List<ItemEffectEntryDto>();
    }

    public class ItemEffectEntryDto
    {
        public string Effect { get; set; } = string.Empty;
        public NamedApiResourceDto Language { get; set; } = new NamedApiResourceDto();
    }
}
