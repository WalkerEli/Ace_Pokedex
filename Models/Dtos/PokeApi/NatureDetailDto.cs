namespace TeamAceProject.Models.Dtos.PokeApi
{
    public class NatureDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public NamedApiResourceDto? Increased_Stat { get; set; }
        public NamedApiResourceDto? Decreased_Stat { get; set; }
        public NamedApiResourceDto? Likes_Flavor { get; set; }
        public NamedApiResourceDto? Hates_Flavor { get; set; }
        public List<NatureNameDto> Names { get; set; } = new();
    }

    public class NatureNameDto
    {
        public string Name { get; set; } = string.Empty;
        public NamedApiResourceDto Language { get; set; } = new NamedApiResourceDto();
    }
}
