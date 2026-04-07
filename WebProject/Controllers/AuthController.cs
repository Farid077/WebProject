using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebProject.DataAccess;
using WebProject.ExternalServices.Interfaces;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

public class AuthController(WebProjectDbContext _context, ISessionService _sessionService, IPasswordHasher<User> _hasher) : Controller
{
    public IActionResult Login()
    {
        if (HttpContext.User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginVM vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return View(vm);

        User? user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(u => u.Username == vm.Username, ct);

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
            new (ClaimTypes.Role, user.RoleId!)
        ];

        foreach (int perm in user.Role!.Permissions)
        {
            claims.Add(new Claim("Permission", perm.ToString()));
        }

        ClaimsPrincipal principal = new (new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken ct = default)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("ClaimTypes 'NameIdentifier' is not found");
        await _sessionService.RevokeAsync(userId, ct);
        await HttpContext.SignOutAsync();
        return Redirect("Login");
    }
}
