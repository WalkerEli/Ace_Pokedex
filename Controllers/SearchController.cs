using Microsoft.AspNetCore.Mvc;

namespace TeamAceProject.Controllers;

public class SearchController : Controller
{
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
