using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers
{
    [Authorize]
    public class UsersController(WebProjectDbContext _context, IPasswordHasher<User> _hasher) : Controller
    {
        
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var users = await _context.Users
            .AsNoTracking()
            .Select(x => 
            new UserManagementViewModel 
            {
                Username = x.Username,
            })
            .ToListAsync(cancellationToken);

            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (vm.Password != vm.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match");
                return View(vm);
            }

            User user = new()
            {
                Username = vm.Username,
            };

            user.PasswordHash = _hasher.HashPassword(user, vm.Password);

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Redirect("Index");
        }

        public async Task<IActionResult> Update(string username, CancellationToken cancellationToken)
        {
            User user = await _getUserAsync(username, cancellationToken);
            UserUpdateViewModel vm = new()
            {
                Username = user.Username,
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string username, UserUpdateViewModel vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (vm.Password != vm.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match");
                return View(vm);
            }

            User user = await _getUserAsync(username, cancellationToken);

            user.Username = vm.Username;
            user.PasswordHash = _hasher.HashPassword(user, vm.Password);
            await _context.SaveChangesAsync(cancellationToken);
            return RedirectToAction("Index", "Users");
        }

        public async Task<IActionResult> Delete(string username, CancellationToken cancellationToken)
        {
            User user = await _getUserAsync(username, cancellationToken);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return RedirectToAction("Index", "Users");
        }
        async Task<User> _getUserAsync(string username, CancellationToken cancellationToken)
        {
            return await _context.Users.FindAsync(username, cancellationToken) ?? throw new Exception("User not found with this username");
        }
    }
}
