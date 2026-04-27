using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Infrastructure;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Posts;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    // Handles the MVC post feed — list, details, create, edit, and delete
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        // Shows all posts in reverse chronological order
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAllPostsAsync();
            return View(posts);
        }

        // Shows a single post with its team roster, reactions, and comments
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

        // Shows the create post form, optionally pre-selecting a team via query string
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

        // Saves the new post and redirects to its details page
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            post.UserId = currentUserId.Value;

            // These are set by the server, not the form — clear any binding errors for them
            ModelState.Remove(nameof(Post.UserId));
            ModelState.Remove(nameof(Post.TeamId));

            if (post.TeamId == Guid.Empty)
                ModelState.AddModelError(nameof(Post.TeamId), "Please select a team.");

            if (!ModelState.IsValid)
                return View(post);

            Post createdPost = await _postService.CreatePostAsync(post);
            return RedirectToAction(nameof(Details), new { id = createdPost.Id });
        }

        // Shows the edit post form pre-filled with the current team and caption
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound();
            if (post.UserId != currentUserId.Value) return Forbid();

            var input = new EditPostInputModel
            {
                Id = post.Id,
                TeamId = post.TeamId,
                Caption = post.Caption,
            };

            return View(input);
        }

        // Saves the updated team and caption, then redirects to the post details page
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPostInputModel input)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            if (input.TeamId == Guid.Empty)
                ModelState.AddModelError(nameof(EditPostInputModel.TeamId), "Please select a team.");

            if (!ModelState.IsValid)
                return View(input);

            bool success = await _postService.EditPostAsync(input, currentUserId.Value);
            if (!success) return Forbid();

            return RedirectToAction(nameof(Details), new { id = input.Id });
        }

        // Deletes the post after verifying the requester is the owner
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound();
            if (post.UserId != currentUserId.Value) return Forbid();

            await _postService.DeletePostAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
