using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.Attributes;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

[AuthorizePermission((int)Pages.Departments, (int)PageAccess.Read)]
public class DepartmentsController(WebProjectDbContext _context) : Controller
{
    // ============ INDEX ============
    [AuthorizePermission((int)Pages.Departments, (int)PageAccess.Read)]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var data = await _context.Departments
            .AsNoTracking()
            .Select(x => new DepartmentManagementVM
            {
                Id = x.Id,
                Name = x.Name,
                Roles = x.Roles.Select(r => r.Name).ToList(),
            })
            .ToListAsync(ct);

        return View(data);
    }

    // ============ CREATE ============
    [AuthorizePermission((int)Pages.Departments, (int)PageAccess.Read_Write)]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Departments, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Create(DepartmentCreateVM vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (await _context.Departments.AnyAsync(x => x.Name.ToLower() == vm.Name.ToLowerInvariant() && x.Name.ToUpper() == vm.Name.ToUpperInvariant() && x.Name == vm.Name, ct))
        {
            ModelState.AddModelError("Name", "This department already exists");
            return View(vm);
        }

        Department Department = new()
        {
            Name = vm.Name,
        };

        await _context.Departments.AddAsync(Department, ct);
        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    // ============ Update ============
    [AuthorizePermission((int)Pages.Departments, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(int id, CancellationToken ct = default)
    {
        Department Department = await _getDepartmentAsync(id, ct);

        DepartmentUpdateVM vm = new()
        {
            Id = Department.Id,
            Name = Department.Name,
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Departments, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(int id, DepartmentUpdateVM vm, CancellationToken ct = default)
    {
        if (id != vm.Id) return BadRequest(ct);

        if (!ModelState.IsValid)
            return View(vm);

        Department Department = await _getDepartmentAsync(id, ct);

        if (Department.Name != vm.Name)
        {
            if (await _context.Departments.AnyAsync(x => x.Name.ToLower() == vm.Name.ToLower(), ct))
            {
                ModelState.AddModelError("Name", "This department already exists");
                return View(vm);
            }
            Department.Name = vm.Name;
        }

        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Departments, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        Department Department = await _getDepartmentAsync(id, ct);

        _context.Departments.Remove(Department);
        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    private async Task<Department> _getDepartmentAsync(int id, CancellationToken ct = default)
    {
        return await _context.Departments.FindAsync(id) ?? throw new Exception($"Department is not found with this id: {id}");
    }
}
