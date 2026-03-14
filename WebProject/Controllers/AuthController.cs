using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebProject.DataAccess;
using WebProject.ExternalServices.Interfaces;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

public class AuthController(WebProjectDbContext _context, ISessionService _sessionService, IPasswordHasher<User> _hasher) : Controller
{
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginViewModel vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return View(vm);

        User? user = await _context.Users.FindAsync(vm.Username, ct);

        if (user == null)
        {
            ModelState.AddModelError("Username", "Username is not found!");
            return View(vm);
        }
        string userId = user.Username;

        var result = await Task.Run(() => _hasher.VerifyHashedPassword(user, user.PasswordHash, vm.Password));

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("Password", "Password is wrong!");
            return View(vm);
        }
        
        string sessionToken = Guid.NewGuid().ToString();

        await _sessionService.CreateAsync(userId, sessionToken, ct);

        List<Claim> claims = 
        [
            new (ClaimTypes.NameIdentifier, userId),
            new ("SessionToken", sessionToken),
        ];

        ClaimsPrincipal principal = new (new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken ct = default)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("ClaimTypes NameIdentifier not found");
        await _sessionService.RevokeAsync(userId, ct);
        await HttpContext.SignOutAsync();
        return Redirect("Login");
    }
}
