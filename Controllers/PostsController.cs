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
        private readonly IPostRepository _postRepo;

        public PostsController(IPostRepository DbPostRepository)
        {
            _postRepo = DbPostRepository;
        }

        // Shows all posts in reverse chronological order
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _postRepo.GetAllPostsAsync();
            return View(posts);
        }

        // Shows a single post with its team roster, reactions, and comments
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _postRepo.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // Shows the create post form, optionally pre-selecting a team via query string
        [Authorize]
        [HttpGet]
        public IActionResult Create(int? teamId)
        {
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            Post post = new Post
            {
                TeamId = teamId ?? 0,
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
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            post.UserId = currentUserId.Value;

            // These are set by the server, not the form — clear any binding errors for them
            ModelState.Remove(nameof(Post.UserId));
            ModelState.Remove(nameof(Post.TeamId));

            if (post.TeamId == 0)
                ModelState.AddModelError(nameof(Post.TeamId), "Please select a team.");

            if (!ModelState.IsValid)
                return View(post);

            Post createdPost = await _postRepo.CreatePostAsync(post);
            return RedirectToAction(nameof(Details), new { id = createdPost.Id });
        }

        // Shows the edit post form pre-filled with the current team and caption
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var post = await _postRepo.GetPostByIdAsync(id);
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
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            if (input.TeamId == 0)
                ModelState.AddModelError(nameof(EditPostInputModel.TeamId), "Please select a team.");

            if (!ModelState.IsValid)
                return View(input);

            bool success = await _postRepo.EditPostAsync(input, currentUserId.Value);
            if (!success) return Forbid();

            return RedirectToAction(nameof(Details), new { id = input.Id });
        }

        // Shows a confirmation page before deleting the post
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postRepo.GetPostByIdAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        // Deletes the post after verifying the requester is the owner
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var post = await _postRepo.GetPostByIdAsync(id);
            if (post == null) return NotFound();
            if (post.UserId != currentUserId.Value) return Forbid();

            await _postRepo.DeletePostAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
