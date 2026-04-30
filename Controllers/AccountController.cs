using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Auth;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers;

// Handles user registration, login, and logout using cookie authentication
public class AccountController : Controller
{
    private readonly IUserRepository _userRepo;

    public AccountController(IUserRepository DbUserRepository)
    {
        _userRepo = DbUserRepository;
    }

    // Shows the registration form, redirects home if already logged in
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterInputModel());
    }

    // Creates a new account, signs the user in, and redirects to their profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterInputModel input)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        User? user = await _userRepo.RegisterUserAsync(input);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "That username or email is already in use.");
            return View(input);
        }

        await SignInUserAsync(user);
        return RedirectToAction("Details", "Users", new { id = user.Id });
    }

    // Shows the login form, redirects home if already logged in
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginInputModel());
    }

    // Authenticates the user and sets a persistent 7-day auth cookie
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel input, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        User? user = await _userRepo.AuthenticateUserAsync(input.UsernameOrEmail, input.Password);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
            return View(input);
        }

        await SignInUserAsync(user);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Details", "Users", new { id = user.Id });
    }

    // Clears the auth cookie and redirects to the home page
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // Builds the claims principal from the user record and writes the auth cookie
    private async Task SignInUserAsync(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
            });
    }
}
