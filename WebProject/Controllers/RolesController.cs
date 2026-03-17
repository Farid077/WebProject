using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

[Authorize]
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
                {{
                    Enum.GetValues<Pages>().FirstOrDefault(x => (p & (int)x) == (int)x).ToString(), 
                    (p & (int)PageAccess.Read_Write) == (int)PageAccess.Read_Write ? PageAccess.Read_Write.ToString() : PageAccess.Read.ToString()

                }}).ToList(),

                Users = r.Users!.Select(u => new RoleUsersViewModel
                {
                    Username = u.Username
                }).ToList()
            })
            .ToListAsync(ct);

        return View(roles);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new RoleCreateViewModel
        {
            PageOptions = Enum.GetNames<Pages>(),
            AccessOptions = Enum.GetNames<PageAccess>(),
            Permissions = [new Pare()]
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoleCreateViewModel vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            vm.PageOptions = Enum.GetNames<Pages>();
            vm.AccessOptions = Enum.GetNames<PageAccess>();
            return View(vm);
        }

        Role role = new()
        {
            Name = vm.RoleName,
            Permissions = vm.Permissions.Select(p => (int)Enum.GetValues<Pages>().FirstOrDefault(e => e.ToString() == p.Page) 
            | (int)Enum.GetValues<PageAccess>().FirstOrDefault(a => a.ToString() == p.Access)).ToList()
        };

        await _context.Roles.AddAsync(role, ct);
        await _context.SaveChangesAsync(ct);

        return Redirect("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddPair(RoleCreateViewModel vm)
    {
        vm.Permissions.Add(new Pare());
        vm.PageOptions = Enum.GetNames<Pages>();
        vm.AccessOptions = Enum.GetNames<PageAccess>();

        return View("Create", vm);
    }

    [HttpPost]
    public async Task<IActionResult> RemovePair(RoleCreateViewModel vm, int index)
    {
        if(vm.Permissions.Count > 1)
            vm.Permissions.RemoveAt(index);

        vm.PageOptions = Enum.GetNames<Pages>();
        vm.AccessOptions = Enum.GetNames<PageAccess>();

        return View("Create", vm);
    }

    public async Task<IActionResult> Update(string id, CancellationToken ct = default)
    {
        var role = await _getRoleAsync(id, ct);



        return View();
    }

    [HttpPost]
    public IActionResult Update(CancellationToken ct = default)
    {


        return View();
    }

    public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
    {
        var role = await _getRoleAsync(id, ct);
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(ct);
        return RedirectToAction("Index", "Roles");
    }

    public async Task<Role> _getRoleAsync(string id, CancellationToken ct = default)
    {
        return await _context.Roles.FindAsync(id, ct) ?? throw new Exception($"Role is not found with this id: {id}");
    }
}
