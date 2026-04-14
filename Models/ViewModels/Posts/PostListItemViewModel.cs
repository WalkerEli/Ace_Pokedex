namespace TeamAceProject.Models.ViewModels.Posts
{
    public class PostListItemViewModel
    {
        public Guid Id { get; set; }
        public string Caption { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
    }
}
