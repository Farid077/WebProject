using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

public class IssueCategoryController(WebProjectDbContext _context) : Controller
{
    // ============ INDEX ============
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var data = await _context.IssueCategories
            .Select(x => new IssueCategoryManagementVM
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync(ct);

        return View(data);
    }

    // ============ DETAILS ============
    //public async Task<IActionResult> Details(int id)
    //{
    //    var data = await _context.IssueCategories
    //        .Where(x => x.Id == id)
    //        .Select(x => new IssueCategoryDetailsVM
    //        {
    //            Id = x.Id,
    //            Name = x.Name
    //        })
    //        .FirstOrDefaultAsync();

    //    if (data == null) return NotFound();

    //    return View(data);
    //}

    // ============ CREATE ============
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IssueCategoryCreateVM vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(vm);

        IssueCategory issueCategory = new()
        {
            Name = vm.Name
        };

        await _context.IssueCategories.AddAsync(issueCategory, ct);
        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    // ============ Update ============
    public async Task<IActionResult> Update(int id, CancellationToken ct = default)
    {
        var category = await _context.IssueCategories.FindAsync(id, ct);
        if (category == null) return NotFound(ct);

        IssueCategoryUpdateVM vm = new()
        {
            Id = category.Id,
            Name = category.Name
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, IssueCategoryUpdateVM vm, CancellationToken ct = default)
    {
        if (id != vm.Id) return BadRequest(ct);

        var category = await _context.IssueCategories.FindAsync(id);
        if (category == null) return NotFound(ct);

        if (!ModelState.IsValid)
            return View(vm);

        category.Name = vm.Name;

        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    // ============ DELETE ============
    //public async Task<IActionResult> Delete(int id)
    //{
    //    var data = await _context.IssueCategories
    //        .Where(x => x.Id == id)
    //        .Select(x => new IssueCategoryDetailsVM
    //        {
    //            Id = x.Id,
    //            Name = x.Name
    //        })
    //        .FirstOrDefaultAsync();

    //    if (data == null) return NotFound();

    //    return View(data);
    //}

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        IssueCategory category = await _getIssueCategoryAsync(id, ct);

        _context.IssueCategories.Remove(category);
        await _context.SaveChangesAsync(ct);
    
        return RedirectToAction(nameof(Index));
    }

    public async Task<IssueCategory> _getIssueCategoryAsync(int id, CancellationToken ct = default)
    {
        return await _context.IssueCategories.FindAsync(id) ?? throw new Exception($"Category is not found with this id: {id}");
    }
}
