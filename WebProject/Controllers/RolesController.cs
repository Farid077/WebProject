using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.Attributes;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

[AuthorizePermission((int)Pages.Roles)]
public class RolesController(WebProjectDbContext _context) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .Select(role => new RoleManagementVM
            {
                Name = role.Name,

                Department = role.Department != null ? role.Department.Name : "-",

                Permissions = role.Permissions.Select(perm => new Dictionary<string, string>
                {{
                    Enum.GetValues<Pages>().FirstOrDefault(page => (perm & (int)page) == (int)page).ToString(),
                    (perm & (int)PageAccess.Read_Write) == (int)PageAccess.Read_Write ? PageAccess.Read_Write.ToString() : PageAccess.Read.ToString()

                }}).ToList(),

                Users = role.Users!.Select(user => user.Username).ToList()
            })
            .ToListAsync(ct);

        return View(roles);
    }

    [AuthorizePermission((int)Pages.Roles, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Create()
    {
        var vm = new RoleCreateVM
        {
            DepartmentOptions = await _context.Departments.Select(d => d.Name).ToListAsync(),
            PageOptions = Enum.GetNames<Pages>(),
            AccessOptions = Enum.GetNames<PageAccess>(),
            Permissions = [new Pair()]
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Roles, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Create(RoleCreateVM vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            vm.DepartmentOptions = await _context.Departments.Select(d => d.Name).ToListAsync();
            vm.PageOptions = Enum.GetNames<Pages>();
            vm.AccessOptions = Enum.GetNames<PageAccess>();
            return View(vm);
        }

        if (await _context.Roles.AnyAsync(x => x.Name.ToLower() == vm.RoleName.ToLower(), ct))
        {
            vm.DepartmentOptions = await _context.Departments.Select(d => d.Name).ToListAsync();
            vm.PageOptions = Enum.GetNames<Pages>();
            vm.AccessOptions = Enum.GetNames<PageAccess>();
            ModelState.AddModelError("RoleName", "This role already exists");
            return View(vm);
        }

        // Eyniləri silmək
        for (int i = 0; i < vm.Permissions.Count - 1; i++)
        {
            for (int j = i + 1; j < vm.Permissions.Count; j++)
            {
                if (vm.Permissions[i].Page == vm.Permissions[j].Page)
                    vm.Permissions.RemoveAt(j);
            }
        }

        Department department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == vm.Department) ?? throw new Exception($"Department is not found with this name: {vm.Department}");

        Role role = new()
        {
            Name = vm.RoleName,
            DepartmentId = department.Id,
            Permissions = [.. vm.Permissions.Select(perm => (int)Enum.GetValues<Pages>().FirstOrDefault(page => page.ToString() == perm.Page)
            | (int)Enum.GetValues<PageAccess>().FirstOrDefault(access => access.ToString() == perm.Access))]
        };

        await _context.Roles.AddAsync(role, ct);
        await _context.SaveChangesAsync(ct);

        return Redirect("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPair(RoleCreateVM vm)
    {
        vm.Permissions.Add(new Pair());
        vm.PageOptions = Enum.GetNames<Pages>();
        vm.AccessOptions = Enum.GetNames<PageAccess>();
        vm.DepartmentOptions = await _context.Departments.Select(d => d.Name).ToListAsync();

        return View("Create", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemovePair(RoleCreateVM vm, int index)
    {
        if(vm.Permissions.Count > 1)
            vm.Permissions.RemoveAt(index);

        vm.PageOptions = Enum.GetNames<Pages>();
        vm.AccessOptions = Enum.GetNames<PageAccess>();
        vm.DepartmentOptions = await _context.Departments.Select(d => d.Name).ToListAsync();

        return View("Create", vm);
    }

    [AuthorizePermission((int)Pages.Roles, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(string id, CancellationToken ct = default)
    {
        var role = await _getRoleAsync(id, ct);

        RoleUpdateVM vm = new()
        {
            RoleName = role.Name,
            Department = role.Department != null ? role.Department.Name : "-",
            Permissions = [.. role.Permissions.Select(perm => new Pair() 
            { 
                Page = Enum.GetValues<Pages>().FirstOrDefault(page => (perm & (int)page) == (int)page).ToString(),
                Access = (perm & (int)PageAccess.Read_Write) == (int)PageAccess.Read_Write ? PageAccess.Read_Write.ToString() : PageAccess.Read.ToString()
            })],
            PageOptions = Enum.GetNames<Pages>(),
            AccessOptions = Enum.GetNames<PageAccess>(),
            DepartmentOptions = await _context.Departments.Select(d => d.Name).ToListAsync(),
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Roles, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(RoleUpdateVM vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            vm.PageOptions = Enum.GetNames<Pages>();
            vm.AccessOptions = Enum.GetNames<PageAccess>();
            vm.DepartmentOptions = await _context.Departments.Select(d => d.Name).ToListAsync();
            return View(vm);
        }

        Role role = await _getRoleAsync(vm.RoleName, ct);

        for (int i = 0; i < vm.Permissions.Count - 1; i++)
        {
            for (int j = i + 1; j < vm.Permissions.Count; j++)
            {
                if (vm.Permissions[i].Page == vm.Permissions[j].Page)
                    vm.Permissions.RemoveAt(j);
            }
        }

        Department department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == vm.Department) ?? throw new Exception($"Department is not found with this name: {vm.Department}");

        role.DepartmentId = department.Id;
        role.Permissions = [.. vm.Permissions.Select(perm => (int)Enum.GetValues<Pages>().FirstOrDefault(page => page.ToString() == perm.Page)
            | (int)Enum.GetValues<PageAccess>().FirstOrDefault(access => access.ToString() == perm.Access))];

        await _context.SaveChangesAsync(ct);
        return Redirect("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPairUpdate(RoleUpdateVM vm)
    {
        vm.Permissions.Add(new Pair());
        vm.PageOptions = Enum.GetNames<Pages>();
        vm.AccessOptions = Enum.GetNames<PageAccess>();
        vm.DepartmentOptions = await _context.Departments.Select(d => d.Name).ToListAsync();

        return View("Update", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemovePairUpdate(RoleUpdateVM vm, int index)
    {
        if (vm.Permissions.Count > 1)
            vm.Permissions.RemoveAt(index);

        vm.PageOptions = Enum.GetNames<Pages>();
        vm.AccessOptions = Enum.GetNames<PageAccess>();
        vm.DepartmentOptions = await _context.Departments.Select(d => d.Name).ToListAsync();

        return View("Update", vm);
    }

    [AuthorizePermission((int)Pages.Roles, (int)PageAccess.Read_Write)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
    {
        var role = await _getRoleAsync(id, ct);
        
        foreach (User user in role.Users)
        {
            user.RoleId = "User";
        }
        
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(ct);
        return RedirectToAction("Index", "Roles");
    }

    private async Task<Role> _getRoleAsync(string id, CancellationToken ct = default)
    {
        return await _context.Roles.FindAsync(id, ct) ?? throw new Exception($"Role is not found with this id: {id}");
    }
}
