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
    }
}
