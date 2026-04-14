using TeamAceProject.Models.Entities;
using TeamAceProject.Models.Enums;
using TeamAceProject.Models.ViewModels.Posts;

namespace TeamAceProject.Services.Interfaces
{
    public interface IPostService
    {
        Task<List<PostListItemViewModel>> GetAllPostsAsync();
        Task<PostDetailsViewModel?> GetPostByIdAsync(Guid postId);
        Task<Post> CreatePostAsync(Post post);
        Task<Comment> AddCommentAsync(Comment comment);
        Task<Reaction> AddOrUpdateReactionAsync(Guid postId, Guid userId, ReactionType reactionType);
    }
}
