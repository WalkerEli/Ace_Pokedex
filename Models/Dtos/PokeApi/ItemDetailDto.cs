namespace TeamAceProject.Models.Dtos.PokeApi
{
    public class ItemDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ItemSpritesDto Sprites { get; set; } = new ItemSpritesDto();
        public NamedApiResourceDto Category { get; set; } = new NamedApiResourceDto();
        public List<ItemNameDto> Names { get; set; } = new List<ItemNameDto>();
        public List<ItemEffectEntryDto> Effect_Entries { get; set; } = new List<ItemEffectEntryDto>();
    }

    public class ItemSpritesDto
    {
        public string? Default { get; set; }
    }

    public class ItemNameDto
    {
        public string Name { get; set; } = string.Empty;
        public NamedApiResourceDto Language { get; set; } = new NamedApiResourceDto();
    }

    public class ItemEffectEntryDto
    {
        public string Effect { get; set; } = string.Empty;
        public string Short_Effect { get; set; } = string.Empty;
        public NamedApiResourceDto Language { get; set; } = new NamedApiResourceDto();
    }

    public class ItemAttributeDto
    {
        public List<NamedApiResourceDto> Items { get; set; } = new List<NamedApiResourceDto>();
    }
}
