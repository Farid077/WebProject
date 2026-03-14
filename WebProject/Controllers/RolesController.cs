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
            var roles = await _context.Roles
                .AsNoTracking()
                .Include(x => x.Users)
                .Select(r => new RoleManagementViewModel
                    {
                        Name = r.Name,

                        Permissions = r.Permissions.Select(p => new Dictionary<string, string>
                        {
                                    { Enum.GetValues<Pages>().FirstOrDefault(x => (p & (int)x) == (int)x).ToString(), (p & (int)Permissions.Read_Write) == (int)Permissions.Read_Write ? Permissions.Read_Write.ToString() : Permissions.Read.ToString()}
                        }).ToList(),

                        Users = r.Users!.Select(u => new RoleUsersViewModel
                        {
                            Username = u.Username
                        }).ToList()
                })
                .ToListAsync(ct);

            //Console.WriteLine(System.Enum.GetNames(typeof(Pages)).Length);
            //Console.WriteLine(System.Enum.GetValues<Pages>().Length);
            //Console.WriteLine(Enum.GetValues<Permissions>().FirstOrDefault(x => (6 & (int)x) == (int)x));

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
