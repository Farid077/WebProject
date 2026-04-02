using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

public class IssuesController(WebProjectDbContext _context) : Controller
{
    // ================= INDEX =================
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var data = await _context.Issues
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Category)
            .Select(x => new IssueManagementVM
            {
                Id = x.Id,
                Title = x.Title,
                Subtitle = x.Subtitle,
                CreatedTime = x.CreatedTime,
                Category = x.Category != null ? x.Category.Name : "-"
            })
            .ToListAsync(ct);

        return View(data);
    }

    // ================= DETAILS =================
    public async Task<IActionResult> Details(int id, CancellationToken ct = default)
    {
        var data = await _context.Issues
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new IssueDetailsVM
            {
                Title = x.Title,
                Subtitle = x.Subtitle,
                Description = x.Description!,
                Category = x.Category != null ? x.Category.Name : "-",
                CreatedTime = x.CreatedTime,
                UpdatedTime = x.UpdatedTime
            })
            .FirstOrDefaultAsync(ct);

        if (data == null) return NotFound(ct);

        return View(data);
    }

    // ================= CREATE =================
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        IssueCreateVM vm = new()
        {
            Categories = await _context.IssueCategories
                .AsNoTracking()
                .Select(x => new IssueCategoryManagementVM
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync(ct)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IssueCreateVM vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = await _context.IssueCategories
                .AsNoTracking()
                .Select(x => new IssueCategoryManagementVM
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync(ct);

            return View(vm);
        }

        Issue issue = new()
        {
            Title = vm.Title,
            Subtitle = vm.Subtitle,
            Description = vm.Description,
            CategoryId = vm.CategoryId,
        };

        await _context.Issues.AddAsync(issue, ct);
        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    // ================= Update =================
    public async Task<IActionResult> Update(int id, CancellationToken ct = default)
    {
        var issue = await _getIssueAsync(id, ct);

        IssueUpdateVM vm = new()
        {
            Id = issue.Id,
            Title = issue.Title,
            Subtitle = issue.Subtitle,
            Description = issue.Description!,
            CategoryId = issue.CategoryId,
            Categories = await _context.IssueCategories
                .AsNoTracking()
                .Select(x => new IssueCategoryManagementVM
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync(ct)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, IssueUpdateVM vm, CancellationToken ct = default)
    {
        if (id != vm.Id) return BadRequest(ct);

        var issue = await _getIssueAsync(id, ct);

        if (!ModelState.IsValid)
        {
            vm.Categories = await _context.IssueCategories
                .AsNoTracking()
                .Select(x => new IssueCategoryManagementVM
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync(ct);
            return View(vm);
        }

        if (issue.Title != vm.Title || issue.Subtitle != vm.Subtitle || issue.Description != vm.Description || issue.CategoryId != vm.CategoryId)
        {
            issue.Title = vm.Title;
            issue.Subtitle = vm.Subtitle;
            issue.Description = vm.Description;
            issue.CategoryId = vm.CategoryId;
            issue.UpdatedTime = DateOnly.FromDateTime(DateTime.Now);
            await _context.SaveChangesAsync(ct);
        }

        return RedirectToAction(nameof(Index));
    }

    // ================= DELETE =================
    //public async Task<IActionResult> Delete(int id)
    //{
    //    var data = await _context.Issues
    //        .Include(x => x.Category)
    //        .Where(x => x.Id == id && !x.IsDeleted)
    //        .Select(x => new IssueDetailsVM
    //        {
    //            Title = x.Title,
    //            Subtitle = x.Subtitle,
    //            CategoryName = x.Category != null ? x.Category.Name : null
    //        })
    //        .FirstOrDefaultAsync();

    //    if (data == null) return NotFound();

    //    return View(data);
    //}

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var issue = await _getIssueAsync(id, ct, true);

        _context.Issues.Remove(issue);
        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="fromAll"> whether only from amoung all or only not deleted part</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async Task<Issue> _getIssueAsync(int id, CancellationToken ct = default, bool fromAll = false)
    {
        if (fromAll)
            return await _context.Issues.FindAsync(id, ct) ?? throw new Exception($"Issue is not found with this id: {id}");
        else
            return await _context.Issues.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, ct) ?? throw new Exception($"Issue is not found with this id: {id}");
    }
}
