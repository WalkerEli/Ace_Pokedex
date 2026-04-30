using TeamAceProject.Models.Entities;
using TeamAceProject.Models.Enums;
using TeamAceProject.Models.ViewModels.Posts;

namespace TeamAceProject.Services.Interfaces
{
    public interface IPostRepository
    {
        Task<List<PostListItemViewModel>> GetAllPostsAsync();
        Task<PostDetailsViewModel?> GetPostByIdAsync(int postId);
        Task<Post> CreatePostAsync(Post post);
        Task<bool> DeletePostAsync(int postId);
        Task<bool> EditPostAsync(EditPostInputModel input, int requestingUserId);
        Task<Comment> AddCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(int commentId, int requestingUserId);
        Task<Comment?> EditCommentAsync(int commentId, int requestingUserId, string body);
        Task<Reaction> AddOrUpdateReactionAsync(int postId, int userId, ReactionType reactionType);
        Task<(int likes, int dislikes)> GetReactionCountsAsync(int postId);
    }
}
