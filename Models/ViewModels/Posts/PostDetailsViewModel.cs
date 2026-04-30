namespace TeamAceProject.Models.ViewModels.Posts
{
    public class PostDetailsViewModel
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
        public string Caption { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public List<PostTeamMemberViewModel> Members { get; set; } = new List<PostTeamMemberViewModel>();
        public List<PostCommentViewModel> Comments { get; set; } = new List<PostCommentViewModel>();
    }

    public class PostTeamMemberViewModel
    {
        public int SlotIndex { get; set; }
        public string PokemonName { get; set; } = string.Empty;
        public string? PokemonSpriteUrl { get; set; }
        public string? AbilityName { get; set; }
        public string? NatureName { get; set; }
        public string? HeldItemName { get; set; }
        public List<string> Moves { get; set; } = new List<string>();
        public List<string> Types { get; set; } = new List<string>();
    }

    public class PostCommentViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
