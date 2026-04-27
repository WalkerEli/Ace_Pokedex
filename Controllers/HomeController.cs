using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models;

namespace TeamAceProject.Controllers;

// Serves the home, privacy, and error pages
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Returns the landing page
    public IActionResult Index()
    {
        return View();
    }

    // Returns the privacy policy page
    public IActionResult Privacy()
    {
        return View();
    }

    // Returns the error page with the current request ID
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
