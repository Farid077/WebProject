using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebProject.Attributes;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

//[AuthorizePermission((int)Pages.Dashboard, (int)PageAccess.Read)]
[Authorize]
public class HomeController(WebProjectDbContext _context) : Controller
{
    //[AuthorizePermission((int)Pages.Dashboard, (int)PageAccess.Read)]
    public async Task<IActionResult> Index(CancellationToken ct = default)
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
            IssueCreateForm = new()
            {
                Urgencies = await _context.Urgencies.AsNoTracking().Select(u => u.Name).ToListAsync(ct),

                Categories = await _context.IssueCategories
                .AsNoTracking()
                .Select(x => new { x.Name, x.SubCategories }).ToDictionaryAsync(y => y.Name, y => y.SubCategories, ct)
            }
        };

        return View(data);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string errorMessage = "Error!")
    {
        return View(new ErrorVM { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = errorMessage ?? "Error!" });
    }
}
