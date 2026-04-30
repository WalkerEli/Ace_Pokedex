namespace TeamAceProject.Models.Dtos.PokeApi
{
    public class TypeDetailDto
    {
        public string Name { get; set; } = string.Empty;
        public TypeDamageRelationsDto Damage_Relations { get; set; } = new();
    }

    public class TypeDamageRelationsDto
    {
        public List<NamedApiResourceDto> Double_Damage_To { get; set; } = new();
        public List<NamedApiResourceDto> Half_Damage_To { get; set; } = new();
        public List<NamedApiResourceDto> No_Damage_To { get; set; } = new();
    }
}
