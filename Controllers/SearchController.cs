using Microsoft.AspNetCore.Mvc;

namespace TeamAceProject.Controllers;

// Handles the Pokemon search bar — redirects directly to the Pokemon details page
public class SearchController : Controller
{
    // Redirects to Pokemon details if a query was provided, otherwise shows the search page
    [HttpGet]
    public IActionResult Index(string? query)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            return RedirectToAction("Details", "Pokemon", new { id = query.Trim().ToLowerInvariant() });
        }

        return View();
    }
}
