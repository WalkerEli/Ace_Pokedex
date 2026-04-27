using Microsoft.EntityFrameworkCore;
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
            var posts = await _context.Posts.AsNoTracking()
                .Include(post => post.Team)
                    .ThenInclude(team => team.TeamMembers)
                .Include(post => post.User)
                .Include(post => post.Reactions)
                .Include(post => post.Comments)
                .OrderByDescending(post => post.CreatedAt)
                .ToListAsync();

            return posts.Select(post => new PostListItemViewModel
            {
                Id = post.Id,
                TeamId = post.TeamId,
                Caption = post.Caption,
                TeamName = post.Team != null ? post.Team.Name : "unknown team",
                UserId = post.UserId,
                Username = post.User != null ? post.User.Username : "unknown user",
                CreatedAt = post.CreatedAt,
                LikeCount = post.Reactions.Count(reaction => reaction.Type == ReactionType.Like),
                DislikeCount = post.Reactions.Count(reaction => reaction.Type == ReactionType.Dislike),
                MemberSpriteUrls = post.Team != null
                    ? post.Team.TeamMembers
                        .OrderBy(m => m.SlotIndex)
                        .Where(m => !string.IsNullOrEmpty(m.PokemonSpriteUrl))
                        .Select(m => m.PokemonSpriteUrl!)
                        .ToList()
                    : new List<string>(),
                CommentCount = post.Comments.Count,
            }).ToList();
        }

        public async Task<PostDetailsViewModel?> GetPostByIdAsync(Guid postId)
        {
            Post? post = await _context.Posts.AsNoTracking()
                .Include(post => post.Team)
                    .ThenInclude(team => team.TeamMembers)
                        .ThenInclude(member => member.TeamMemberMoves)
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
                Members = post.Team?.TeamMembers
                    .OrderBy(member => member.SlotIndex)
                    .Select(member => new PostTeamMemberViewModel
                    {
                        SlotIndex = member.SlotIndex,
                        PokemonName = ToDisplayName(member.PokemonName),
                        PokemonSpriteUrl = member.PokemonSpriteUrl,
                        AbilityName = ToDisplayName(member.AbilityName),
                        NatureName = ToDisplayName(member.NatureName),
                        HeldItemName = ToDisplayName(member.HeldItemName),
                        Moves = member.TeamMemberMoves
                            .OrderBy(move => move.MoveSlot)
                            .Select(move => ToDisplayName(move.MoveName))
                            .ToList(),
                    })
                    .ToList() ?? new List<PostTeamMemberViewModel>(),
                Comments = post.Comments.OrderByDescending(comment => comment.CreatedAt)
                    .Select(comment => new PostCommentViewModel
                    {
                        Id = comment.Id,
                        UserId = comment.UserId,
                        Username = comment.User?.Username ?? "unknown user",
                        Body = comment.Body,
                        CreatedAt = comment.CreatedAt,
                    })
                    .ToList(),
            };
        }

        private static string ToDisplayName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            return name.Replace("-", " ")
                .Split(' ')
                .Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w[1..] : w)
                .Aggregate((a, b) => $"{a} {b}");
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            Post? post = await _context.Posts.FindAsync(postId);
            if (post == null) return false;
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditPostAsync(EditPostInputModel input, Guid requestingUserId)
        {
            Post? post = await _context.Posts.FindAsync(input.Id);
            if (post == null || post.UserId != requestingUserId) return false;
            post.TeamId = input.TeamId;
            post.Caption = input.Caption.Trim();
            await _context.SaveChangesAsync();
            return true;
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

        public async Task<bool> DeleteCommentAsync(Guid commentId, Guid requestingUserId)
        {
            Comment? comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.UserId != requestingUserId) return false;
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Comment?> EditCommentAsync(Guid commentId, Guid requestingUserId, string body)
        {
            Comment? comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.UserId != requestingUserId) return null;
            comment.Body = body.Trim();
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<(int likes, int dislikes)> GetReactionCountsAsync(Guid postId)
        {
            var reactions = await _context.Reactions
                .Where(r => r.PostId == postId)
                .ToListAsync();
            return (
                reactions.Count(r => r.Type == ReactionType.Like),
                reactions.Count(r => r.Type == ReactionType.Dislike)
            );
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
