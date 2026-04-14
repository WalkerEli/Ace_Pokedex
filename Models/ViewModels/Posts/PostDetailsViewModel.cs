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
        public List<PostCommentViewModel> Comments { get; set; } = new List<PostCommentViewModel>();
    }

    public class PostCommentViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
