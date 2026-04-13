using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebProject.Attributes;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

[AuthorizePermission((int)Pages.Issues, (int)PageAccess.Read)]
public class IssuesController(WebProjectDbContext _context, HttpContext _httpContext) : Controller
{
    // ================= INDEX =================
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var data = await _context.Issues
            .AsNoTracking()
            .Include(x => x.Reporter)
            .Include(x => x.Assignee)
            .Include(x => x.Urgency)
            .Where(x => !x.IsDeleted)
            .Select(x => new IssueManagementVM
            {
                Id = x.Id,
                CreatedTime = x.CreatedTime,
                Category = x.Category,
                SubCategory = x.SubCategory,
                Status = x.Status,
                Urgency = x.Urgency != null ? x.Urgency.Name : "-",
                ReporterName = x.ReporterId ?? "-",
                AssigneeName = x.AssigneeId ?? "-",
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
                Category = x.Category,
                SubCategory = x.SubCategory,
                Description = x.Description,
                CreatedTime = x.CreatedTime,
                UpdatedTime = x.UpdatedTime,
            })
            .FirstOrDefaultAsync(ct);

        if (data == null) return NotFound(ct);

        return View(data);
    }

    // ================= CREATE =================
    [AuthorizePermission((int)Pages.Issues, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        IssueCreateVM vm = new()
        {
            Statuses = Enum.GetNames<IssueStatuses>(),

            Urgencies = await _context.Urgencies.AsNoTracking().Select(u => u.Name).ToListAsync(ct),

            Categories = await _context.IssueCategories
                .AsNoTracking()
                .Select(x => new { x.Name, x.SubCategories }).ToDictionaryAsync(y => y.Name, y => y.SubCategories, ct)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Issues, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Create(IssueCreateVM vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            vm.Statuses = Enum.GetNames<IssueStatuses>();

            vm.Urgencies = await _context.Urgencies.AsNoTracking().Select(u => u.Name).ToListAsync(ct);

            vm.Categories = await _context.IssueCategories
                .AsNoTracking()
                .Select(x => new { x.Name, x.SubCategories }).ToDictionaryAsync(y => y.Name, y => y.SubCategories, ct);

            return View(vm);
        }

        string userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("ClaimTypes NameIdentifier is not found");

        Urgency Urgency = await _context.Urgencies.FirstOrDefaultAsync(u => u.Name == vm.Urgency) ?? throw new Exception($"Urgency is not found with this name: {vm.Urgency}"); ;

        Issue issue = new()
        {
            Category = vm.Category,
            SubCategory = vm.SubCategory,
            Status = vm.Status,
            Description = vm.Description,
            ReporterId = userId,
            UrgencyId = Urgency.Id,
        };

        await _context.Issues.AddAsync(issue, ct);
        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    // ================= Update =================
    [AuthorizePermission((int)Pages.Issues, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(int id, CancellationToken ct = default)
    {
        var issue = await _getIssueAsync(id, ct);

        IssueUpdateVM vm = new()
        {
            Id = issue.Id,
            Description = issue.Description!,
            //CategoryId = issue.CategoryId,
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
    [AuthorizePermission((int)Pages.Issues, (int)PageAccess.Read_Write)]
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

        //if (issue.Title != vm.Title || issue.Subtitle != vm.Subtitle || issue.Description != vm.Description || issue.CategoryId != vm.CategoryId)
        //{
        //    issue.Title = vm.Title;
        //    issue.Subtitle = vm.Subtitle;
        //    issue.Description = vm.Description;
        //    issue.CategoryId = vm.CategoryId;
        //    issue.UpdatedTime = DateOnly.FromDateTime(DateTime.Now);
        //    await _context.SaveChangesAsync(ct);
        //}

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
    [AuthorizePermission((int)Pages.Issues, (int)PageAccess.Read_Write)]
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
