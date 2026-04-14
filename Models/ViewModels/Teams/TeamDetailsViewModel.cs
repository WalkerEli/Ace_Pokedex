namespace TeamAceProject.Models.ViewModels.Teams
{
    public class TeamDetailsViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<TeamMemberViewModel> Members { get; set; } = new List<TeamMemberViewModel>();
        public AddTeamMemberInputModel NewMember { get; set; } = new AddTeamMemberInputModel();
    }

    public class TeamMemberViewModel
    {
        public Guid Id { get; set; }
        public int SlotIndex { get; set; }
        public int PokemonId { get; set; }
        public string PokemonName { get; set; } = string.Empty;
        public string? PokemonSpriteUrl { get; set; }
        public string? AbilityName { get; set; }
        public string? NatureName { get; set; }
        public string? HeldItemName { get; set; }
        public List<string> Moves { get; set; } = new List<string>();
    }
}
