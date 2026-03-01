using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers
{
    [Authorize]
    public class UsersController(WebProjectDbContext _context) : Controller
    {
        
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var users = await _context.Users.Select(x => 
            new UserManagementViewModel 
            { 
                Id = x.Id,
                Username = x.Username, 
                PasswordHash = x.PasswordHash,
            })
            .ToListAsync(cancellationToken);

            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Redirect("Index");
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await _getByIdAsync(id, cancellationToken);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Redirect("../Index");
        }
        async Task<User> _getByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Users.FindAsync(id, cancellationToken) ?? throw new Exception("User not found with this id");
        }
    }
}
