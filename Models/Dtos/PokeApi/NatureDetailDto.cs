namespace TeamAceProject.Models.Dtos.PokeApi
{
    public class NatureDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public NamedApiResourceDto? Increased_Stat { get; set; }
        public NamedApiResourceDto? Decreased_Stat { get; set; }
    }
}
