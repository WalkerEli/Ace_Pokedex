using Microsoft.EntityFrameworkCore;
using TeamAceProject.Data;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.Enums;
using TeamAceProject.Models.ViewModels.Posts;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PostListItemViewModel>> GetAllPostsAsync()
        {
            return await _context.Posts.AsNoTracking()
                .Include(post => post.Team)
                .Include(post => post.User)
                .Include(post => post.Reactions)
                .OrderByDescending(post => post.CreatedAt)
                .Select(post => new PostListItemViewModel
                {
                    Id = post.Id,
                    Caption = post.Caption,
                    TeamName = post.Team != null ? post.Team.Name : "unknown team",
                    Username = post.User != null ? post.User.Username : "unknown user",
                    CreatedAt = post.CreatedAt,
                    LikeCount = post.Reactions.Count(reaction => reaction.Type == ReactionType.Like),
                    DislikeCount = post.Reactions.Count(reaction => reaction.Type == ReactionType.Dislike),
                })
                .ToListAsync();
        }

        public async Task<PostDetailsViewModel?> GetPostByIdAsync(Guid postId)
        {
            Post? post = await _context.Posts.AsNoTracking()
                .Include(post => post.Team)
                .Include(post => post.User)
                .Include(post => post.Reactions)
                .Include(post => post.Comments)
                    .ThenInclude(comment => comment.User)
                .FirstOrDefaultAsync(post => post.Id == postId);

            if (post == null)
            {
                return null;
            }

            return new PostDetailsViewModel
            {
                Id = post.Id,
                TeamId = post.TeamId,
                UserId = post.UserId,
                Caption = post.Caption,
                TeamName = post.Team?.Name ?? "unknown team",
                Username = post.User?.Username ?? "unknown user",
                CreatedAt = post.CreatedAt,
                LikeCount = post.Reactions.Count(reaction => reaction.Type == ReactionType.Like),
                DislikeCount = post.Reactions.Count(reaction => reaction.Type == ReactionType.Dislike),
                Comments = post.Comments.OrderByDescending(comment => comment.CreatedAt)
                    .Select(comment => new PostCommentViewModel
                    {
                        Username = comment.User?.Username ?? "unknown user",
                        Body = comment.Body,
                        CreatedAt = comment.CreatedAt,
                    })
                    .ToList(),
            };
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            post.Id = Guid.NewGuid();
            post.CreatedAt = DateTime.UtcNow;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            comment.Id = Guid.NewGuid();
            comment.CreatedAt = DateTime.UtcNow;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Reaction> AddOrUpdateReactionAsync(Guid postId, Guid userId, ReactionType reactionType)
        {
            Reaction? existingReaction = await _context.Reactions.FindAsync(postId, userId);

            if (existingReaction == null)
            {
                existingReaction = new Reaction
                {
                    PostId = postId,
                    UserId = userId,
                    Type = reactionType,
                    CreatedAt = DateTime.UtcNow,
                };
                _context.Reactions.Add(existingReaction);
            }
            else
            {
                existingReaction.Type = reactionType;
                existingReaction.CreatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return existingReaction;
        }
    }
}
