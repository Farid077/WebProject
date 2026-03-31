using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebProject.Attributes;
using WebProject.DataAccess;
using WebProject.ExternalServices.Interfaces;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

[AuthorizePermission((int)Pages.Users)]
public class UsersController(WebProjectDbContext _context, ISessionService _sessionService, IPasswordHasher<User> _hasher) : Controller
{
    
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var users = await _context.Users
        .AsNoTracking()
        .Include(x => x.Role)
        .Select(x =>
        new UserManagementViewModel
        {
            Username = x.Username,
            Role = x.RoleId!,
            CreatedTime = x.CreatedTime,
        })
        .ToListAsync(ct);

        await Task.WhenAll(
            users.Select(async x => x.IsActive = await _sessionService.IsActiveAsync(x.Username, ct))
        );

        return View(users);
    }

    [AuthorizePermission((int)Pages.Users, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        UserCreateViewModel vm = new()
        {
            RoleOptions = await _context.Roles.Select(r => r.Name).ToListAsync(ct)
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserCreateViewModel vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            vm.RoleOptions = await _context.Roles.Select(r => r.Name).ToListAsync(ct);
            return View(vm);
        }

        if (vm.Password != vm.ConfirmPassword)
        {
            vm.RoleOptions = await _context.Roles.Select(r => r.Name).ToListAsync(ct);
            ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match");
            return View(vm);
        }

        if (await _context.Users.AnyAsync(x => x.Username.ToLower() == vm.Username.ToLower(), ct))
        {
            vm.RoleOptions = await _context.Roles.Select(r => r.Name).ToListAsync(ct);
            ModelState.AddModelError("Username", "This username already exists");
            return View(vm);
        }

        User user = new()
        {
            Username = vm.Username,
            RoleId = vm.Role.IsNullOrEmpty() ? "User" : vm.Role
        };

        user.PasswordHash = await Task.Run(() => _hasher.HashPassword(user, vm.Password));

        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);
        return Redirect("Index");
    }

    [AuthorizePermission((int)Pages.Users, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(string id, CancellationToken ct = default)
    {
        User user = await _getUserAsync(id, ct);

        UserUpdateViewModel vm = new()
        {
            RoleOptions = await _context.Roles.Select(r => r.Name).ToListAsync(ct),
            Role = user.RoleId!
        };

        ViewData["Id"] = id;
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(string id, UserUpdateViewModel vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            vm.RoleOptions = await _context.Roles.Select(r => r.Name).ToListAsync(ct);
            ViewData["Id"] = id;
            return View(vm);
        }

        if(vm.Password != null) {
            if (vm.Password != vm.ConfirmPassword)
            {
                vm.RoleOptions = await _context.Roles.Select(r => r.Name).ToListAsync(ct);
                ViewData["Id"] = id;
                ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match");
                return View(vm);
            }
        }

        User user = await _getUserAsync(id, ct);

        if(vm.Password != null)
        {
            user.PasswordHash = await Task.Run(() => _hasher.HashPassword(user, vm.Password));
            user.UpdatedTime = DateOnly.FromDateTime(DateTime.Now);
        }

        if (vm.Role != user.RoleId && !vm.Role.IsNullOrEmpty())
        {
            user.RoleId = vm.Role;
            user.UpdatedTime = DateOnly.FromDateTime(DateTime.Now);
        }

        user.UpdatedTime = DateOnly.FromDateTime(DateTime.Now);

        await _sessionService.RevokeAsync(id, ct);
        await _context.SaveChangesAsync(ct);
        return RedirectToAction("Index", "Users");
    }

    [AuthorizePermission((int)Pages.Users, (int)PageAccess.Read_Write)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
    {
        User user = await _getUserAsync(id, ct);
        await _sessionService.RevokeAsync(id, ct);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(ct);
        return RedirectToAction("Index", "Users");
    }

    [AuthorizePermission((int)Pages.Users, (int)PageAccess.Read_Write)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RevokeSession(string id, CancellationToken ct = default)
    {
        var user = await _getUserAsync(id, ct);
        await _sessionService.RevokeAsync(id, ct);
        return RedirectToAction("Index", "Users");
    }

    private async Task<User> _getUserAsync(string id, CancellationToken ct = default)
    {
        return await _context.Users.FindAsync(id, ct) ?? throw new Exception($"User not found with this id: {id}");
    }
}
