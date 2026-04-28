namespace TeamAceProject.Models.Dtos.PokeApi
{
    public class MoveDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? Power { get; set; }
        public int? Accuracy { get; set; }
        public int PP { get; set; }
        public NamedApiResourceDto Type { get; set; } = new NamedApiResourceDto();
        public NamedApiResourceDto Damage_Class { get; set; } = new NamedApiResourceDto();
        public List<MoveNameDto> Names { get; set; } = new();
        public List<MoveEffectEntryDto> Effect_Entries { get; set; } = new();
    }

    public class MoveNameDto
    {
        public string Name { get; set; } = string.Empty;
        public NamedApiResourceDto Language { get; set; } = new NamedApiResourceDto();
    }

    public class MoveEffectEntryDto
    {
        public string Effect { get; set; } = string.Empty;
        public string Short_Effect { get; set; } = string.Empty;
        public NamedApiResourceDto Language { get; set; } = new NamedApiResourceDto();
    }
}
