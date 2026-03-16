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

        public async Task<IActionResult> Create()
        {
            var vm = new RoleCreateViewModel
            {
                PageOptions = Enum.GetNames<Pages>(),
                PermissionOptions = Enum.GetNames<Permissions>(),
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(RoleCreateViewModel vm, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                vm.PageOptions = Enum.GetNames<Pages>();
                vm.PermissionOptions = Enum.GetNames<Permissions>();
                return View(vm);
            }


            return Redirect("Index");
        }

        public async Task<IActionResult> Update(string id, CancellationToken ct)
        {
            var role = await _getRoleAsync(id, ct);

            return View();
        }

        [HttpPost]
        public IActionResult Update(CancellationToken ct)
        {


            return View();
        }

        public async Task<Role> _getRoleAsync(string id, CancellationToken ct = default)
        {
            return await _context.Roles.FindAsync(id, ct) ?? throw new Exception($"Role is not found with this id: {id}");
        }
    }
}
