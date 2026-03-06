using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

public class AuthController(WebProjectDbContext _context, IPasswordHasher<User> _hasher) : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginViewModel vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var user = await _context.Users.FindAsync(vm.Username, cancellationToken);

        if (user == null)
        {
            ModelState.AddModelError("", "Username is not found!");
            return View(vm);
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, vm.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Password is wrong!");
            return View(vm);
        }
            

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        _ = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        _ = HttpContext.SignOutAsync();
        return Redirect("Login");
    }
}
