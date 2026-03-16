using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.DataAccess;
using WebProject.ExternalServices.Interfaces;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers
{
    [Authorize]
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

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel vm, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(vm);

            if (vm.Password != vm.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match");
                return View(vm);
            }

            if (await _context.Users.AnyAsync(x => x.Username == vm.Username))
            {
                ModelState.AddModelError("Username", "This username already exists");
                return View(vm);
            }

            User user = new()
            {
                Username = vm.Username,
            };

            user.PasswordHash = await Task.Run(() => _hasher.HashPassword(user, vm.Password));

            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
            return Redirect("Index");
        }

        public async Task<IActionResult> Update(string id)
        {
            //User user = await _getUserAsync(id, ct);
            //UserUpdateViewModel vm = new()
            //{
            //    Username = user.Username,
            //};
            ViewData["Id"] = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, UserUpdateViewModel vm, CancellationToken ct = default)
        {
            //vm.Username = id;

            if (!ModelState.IsValid)
            {
                ViewData["Id"] = id;
                return View(vm);
            }

            if(vm.Password != null) { 
                if (vm.Password != vm.ConfirmPassword)
                {
                    ViewData["Id"] = id;
                    ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match");
                    return View(vm);
                }
            }
            else
                return RedirectToAction("Index", "Users");

            User user = await _getUserAsync(id, ct);

            if(vm.Password != null)
            {
                user.PasswordHash = await Task.Run(() => _hasher.HashPassword(user, vm.Password));
                await _context.SaveChangesAsync(ct);
            }
            //else if(vm.Username == user.Username)
            //    return RedirectToAction("Index", "Users");

            //user.Username = vm.Username;
            return RedirectToAction("Index", "Users");
        }

        public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
        {
            User user = await _getUserAsync(id, ct);
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("ClaimTypes NameIdentifier not found");
            await _sessionService.RevokeAsync(id, ct);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(ct);
            return RedirectToAction("Index", "Users");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RevokeSession(string id, CancellationToken ct = default)
        //{
        //    var user = await _getUserAsync(id, ct);
        //    //if (user is null) return NotFound();

        //    await _sessionService.RevokeAsync(id, ct);

        //    //TempData["Message"] = $"{user.UserName}'s session has been revoked.";
        //    return RedirectToAction("Index", "Users");
        //}

        async Task<User> _getUserAsync(string id, CancellationToken ct = default)
        {
            return await _context.Users.FindAsync(id, ct) ?? throw new Exception($"User not found with this id: {id}");
        }
    }
}
