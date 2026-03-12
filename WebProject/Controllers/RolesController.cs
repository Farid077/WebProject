using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers
{
    public class RolesController(WebProjectDbContext _context) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            //ICollection<Role> roles = await _context.Roles.ToListAsync(ct);

            ICollection<Role> roles = (ICollection<Role>)await _context.Roles.Select(r => new RoleManagementViewModel
            {
                Name = r.Name,
                Permissions = r.Permissions,
                Users = r.Users == null ? new() : r.Users.Select(u => new UserManagementViewModel
                {
                    Username = u.Username
                }).ToList()
            }).ToListAsync(ct);

            return View(roles);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(CancellationToken ct = default)
        {
            return View();
        }
    }
}
