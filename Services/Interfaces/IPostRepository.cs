using TeamAceProject.Models.Entities;
using TeamAceProject.Models.Enums;
using TeamAceProject.Models.ViewModels.Posts;

namespace TeamAceProject.Services.Interfaces
{
    public interface IPostRepository
    {
        Task<List<PostListItemViewModel>> GetAllPostsAsync();
        Task<PostDetailsViewModel?> GetPostByIdAsync(Guid postId);
        Task<Post> CreatePostAsync(Post post);
        Task<bool> DeletePostAsync(Guid postId);
        Task<bool> EditPostAsync(EditPostInputModel input, Guid requestingUserId);
        Task<Comment> AddCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(Guid commentId, Guid requestingUserId);
        Task<Comment?> EditCommentAsync(Guid commentId, Guid requestingUserId, string body);
        Task<Reaction> AddOrUpdateReactionAsync(Guid postId, Guid userId, ReactionType reactionType);
        Task<(int likes, int dislikes)> GetReactionCountsAsync(Guid postId);
    }
}
