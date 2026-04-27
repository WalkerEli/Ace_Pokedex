using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Infrastructure;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.Enums;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    // JSON-only API endpoints consumed by post.js for reactions and comment CRUD
    [ApiController]
    [Route("api/posts")]
    public class PostsApiController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsApiController(IPostService postService)
        {
            _postService = postService;
        }

        // Adds or updates the current user's like/dislike and returns the new counts
        [Authorize]
        [HttpPost("react")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> React([FromForm] Guid postId, [FromForm] ReactionType type)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue) return Unauthorized();

            await _postService.AddOrUpdateReactionAsync(postId, currentUserId.Value, type);
            var (likes, dislikes) = await _postService.GetReactionCountsAsync(postId);
            return Ok(new { likes, dislikes });
        }

        // Saves a new comment and returns its username, body, and timestamp
        [Authorize]
        [HttpPost("add-comment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([FromForm] Guid postId, [FromForm] string body)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue) return Unauthorized();

            if (string.IsNullOrWhiteSpace(body))
                return BadRequest(new { error = "Comment cannot be empty." });

            Comment comment = new Comment
            {
                PostId = postId,
                UserId = currentUserId.Value,
                Body = body.Trim(),
            };

            Comment saved = await _postService.AddCommentAsync(comment);
            return Ok(new { username = User.Identity!.Name, body = saved.Body, createdAt = saved.CreatedAt.ToLocalTime().ToString("g") });
        }

        // Deletes a comment after verifying the requester owns it
        [Authorize]
        [HttpPost("delete-comment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment([FromForm] Guid id)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue) return Unauthorized();

            bool deleted = await _postService.DeleteCommentAsync(id, currentUserId.Value);
            if (!deleted) return Forbid();

            return Ok(new { success = true });
        }

        // Updates a comment's body text after verifying the requester owns it
        [Authorize]
        [HttpPost("edit-comment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment([FromForm] Guid id, [FromForm] string body)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue) return Unauthorized();

            if (string.IsNullOrWhiteSpace(body))
                return BadRequest(new { error = "Comment cannot be empty." });

            Comment? updated = await _postService.EditCommentAsync(id, currentUserId.Value, body);
            if (updated == null) return Forbid();

            return Ok(new { body = updated.Body });
        }
    }
}
