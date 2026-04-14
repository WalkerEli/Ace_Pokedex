using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Infrastructure;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.Enums;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAllPostsAsync();
            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create(Guid? teamId)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            Post post = new Post
            {
                TeamId = teamId ?? Guid.Empty,
                UserId = currentUserId.Value,
            };

            return View(post);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            post.UserId = currentUserId.Value;

            if (!ModelState.IsValid)
            {
                return View(post);
            }

            Post createdPost = await _postService.CreatePostAsync(post);
            return RedirectToAction(nameof(Details), new { id = createdPost.Id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Guid postId, string body)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                return RedirectToAction(nameof(Details), new { id = postId });
            }

            Comment comment = new Comment
            {
                PostId = postId,
                UserId = currentUserId.Value,
                Body = body.Trim(),
            };

            await _postService.AddCommentAsync(comment);
            return RedirectToAction(nameof(Details), new { id = postId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> React(Guid postId, ReactionType type)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            await _postService.AddOrUpdateReactionAsync(postId, currentUserId.Value, type);
            return RedirectToAction(nameof(Details), new { id = postId });
        }
    }
}
