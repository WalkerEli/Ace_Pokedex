namespace TeamAceProject.Models.ViewModels.Teams
{
    public class TeamListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public int MemberCount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
