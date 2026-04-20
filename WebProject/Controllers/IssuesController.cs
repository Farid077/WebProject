using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebProject.Attributes;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

[AuthorizePermission((int)Pages.Issues, (int)PageAccess.Read)]
public class IssuesController(WebProjectDbContext _context) : Controller
{
    // ================= INDEX =================
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var data = await _context.Issues
            .AsNoTracking()
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
    public async Task<IActionResult> Create(IssueCreateVM vm, string? returnUrl, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            vm.Urgencies = await _context.Urgencies.AsNoTracking().Select(u => u.Name).ToListAsync(ct);

            vm.Categories = await _context.IssueCategories
                .AsNoTracking()
                .Select(x => new { x.Name, x.SubCategories }).ToDictionaryAsync(y => y.Name, y => y.SubCategories, ct);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                HomePageVM data = new()
                {
                    Issues = await _context.Issues
                    .AsNoTracking()
                    .Where(i => !i.IsDeleted)
                    .Select(i => new IssueCardVM
                    {
                        Id = i.Id,
                        Category = i.Category,
                        SubCategory = i.SubCategory,
                        Description = i.Description,
                        Status = i.Status,
                        Urgency = i.Urgency != null ? i.Urgency.Name : "",
                        ReportedTime = i.CreatedTime,
                        ReporterName = i.ReporterId!,
                        AssigneeName = i.AssigneeId ?? "",
                    })
                    .ToListAsync(ct),
                    IssueCreateForm = vm,
                };
                return View("~/Views/Home/Index.cshtml", data);
            }

            return View(vm);
        }
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("ClaimTypes NameIdentifier is not found");

        Urgency urgency = await _context.Urgencies.FirstOrDefaultAsync(u => u.Name == vm.Urgency) ?? throw new Exception($"Urgency is not found with this name: {vm.Urgency}"); ;

        Issue issue = new()
        {
            Category = vm.Category,
            SubCategory = vm.SubCategory,
            Description = vm.Description!,
            Status = IssueStatuses.Pending.ToString(),
            ReporterId = userId,
            UrgencyId = urgency.Id,
        };

        await _context.Issues.AddAsync(issue, ct);
        await _context.SaveChangesAsync(ct);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    // ================= Update =================
    [AuthorizePermission((int)Pages.Issues, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(int id, CancellationToken ct = default)
    {
        IssueUpdateVM vm = await _context.Issues
            .Where(i => !i.IsDeleted && i.Id == id)
            .Select(i => new IssueUpdateVM
            {
                Id = i.Id,
                Category = i.Category,
                SubCategory = i.SubCategory,
                Description = i.Description,
                Status = i.Status,
                Urgency = i.Urgency != null ? i.Urgency.Name : "-",
                AssigneeId = i.AssigneeId ?? "-",
                Statuses = Enum.GetNames<IssueStatuses>(),
            })
            .FirstOrDefaultAsync(ct) ?? throw new Exception($"Issue is not found with this Id: {id}");

        vm.Urgencies = await _context.Urgencies.AsNoTracking().Select(u => u.Name).ToListAsync(ct);
        vm.Users = await _context.Users.AsNoTracking().Select(u => u.Username).ToListAsync(ct);
        vm.Categories = await _context.IssueCategories.AsNoTracking()
                .Select(x => new { x.Name, x.SubCategories }).ToDictionaryAsync(y => y.Name, y => y.SubCategories, ct);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Issues, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(int id, string returnUrl, IssueUpdateVM vm, CancellationToken ct = default)
    {
        if (id != vm.Id) return BadRequest(ct);

        if (!ModelState.IsValid)
        {
            vm.Statuses = Enum.GetNames<IssueStatuses>();
            vm.Urgencies = await _context.Urgencies.AsNoTracking().Select(u => u.Name).ToListAsync(ct);
            vm.Users = await _context.Users.AsNoTracking().Select(u => u.Username).ToListAsync(ct);
            vm.Categories = await _context.IssueCategories.AsNoTracking()
                    .Select(x => new { x.Name, x.SubCategories }).ToDictionaryAsync(y => y.Name, y => y.SubCategories, ct);
            return View(vm);
        }

        Issue issue = await _getIssueAsync(id, ct);

        Urgency urgency = await _context.Urgencies.FirstOrDefaultAsync(u => u.Name == vm.Urgency) ?? throw new Exception($"Urgency is not found with this name: {vm.Urgency}");

        issue.Category = vm.Category;
        issue.SubCategory = vm.SubCategory;
        issue.Description = vm.Description ?? "";
        issue.Status = vm.Status;
        issue.UrgencyId = urgency.Id;
        issue.AssigneeId = vm.AssigneeId;
        
        issue.UpdatedTime = DateTime.UtcNow.AddHours(4);

        await _context.SaveChangesAsync(ct);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Issues, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Delete(int id, string? returnUrl, CancellationToken ct = default)
    {
        var issue = await _getIssueAsync(id, ct, true);

        _context.Issues.Remove(issue);
        await _context.SaveChangesAsync(ct);
        Console.WriteLine(returnUrl);
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

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
