using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.Attributes;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

[AuthorizePermission((int)Pages.IssueCategories, (int)PageAccess.Read)]
public class IssueCategoriesController(WebProjectDbContext _context) : Controller
{
    // ============ INDEX ============
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var data = await _context.IssueCategories
            .AsNoTracking()
            .Select(x => new IssueCategoryManagementVM
            {
                Id = x.Id,
                Name = x.Name,
                SubCategories = x.SubCategories,
            })
            .ToListAsync(ct);

        return View(data);
    }

    // ============ CREATE ============
    [AuthorizePermission((int)Pages.IssueCategories, (int)PageAccess.Read_Write)]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.IssueCategories, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Create(IssueCategoryCreateVM vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (await _context.IssueCategories.AnyAsync(x => x.Name.ToLower() == vm.Name.ToLower(), ct))
        {
            ModelState.AddModelError("Name", "This category already exists");
            return View(vm);
        }

        vm.SubCategories.Add("Other");

        IssueCategory issueCategory = new()
        {
            Name = vm.Name,
            SubCategories = vm.SubCategories,
        };

        await _context.IssueCategories.AddAsync(issueCategory, ct);
        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    // ============ Update ============
    [AuthorizePermission((int)Pages.IssueCategories, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(int id, CancellationToken ct = default)
    {
        IssueCategory category = await _getIssueCategoryAsync(id, ct);

        IssueCategoryUpdateVM vm = new()
        {
            Id = category.Id,
            Name = category.Name,
            SubCategories = category.SubCategories ?? [],
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.IssueCategories, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(int id, IssueCategoryUpdateVM vm, CancellationToken ct = default)
    {
        if (id != vm.Id) return BadRequest(ct);

        if (!ModelState.IsValid)
            return View(vm);

        IssueCategory category = await _getIssueCategoryAsync(id, ct);

        if(category.Name != vm.Name)
        {
            if (await _context.IssueCategories.AnyAsync(x => x.Name.ToLower() == vm.Name.ToLower(), ct))
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View(vm);
            }
            category.Name = vm.Name;
        }

        vm.SubCategories.Add("Other");

        category.SubCategories = vm.SubCategories;

        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.IssueCategories, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        IssueCategory category = await _getIssueCategoryAsync(id, ct);

        _context.IssueCategories.Remove(category);
        await _context.SaveChangesAsync(ct);
    
        return RedirectToAction(nameof(Index));
    }

    private async Task<IssueCategory> _getIssueCategoryAsync(int id, CancellationToken ct = default)
    {
        return await _context.IssueCategories.FindAsync(id) ?? throw new Exception($"Category is not found with this id: {id}");
    }
}
