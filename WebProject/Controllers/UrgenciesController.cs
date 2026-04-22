using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject.Attributes;
using WebProject.DataAccess;
using WebProject.Models;
using WebProject.ViewModels;

namespace WebProject.Controllers;

[AuthorizePermission((int)Pages.Urgencies, (int)PageAccess.Read)]
public class UrgenciesController(WebProjectDbContext _context) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var data = await _context.Urgencies
            .AsNoTracking()
            .Select(u => new UrgencyManagementVM
            {
                Id = u.Id,
                Name = u.Name,
                Days = u.Time/(24 * 60),
                Hours = (u.Time - (u.Time/(24 * 60) * 24 * 60))/60,
                Minutes = u.Time % 60,
            })
            .ToListAsync(ct);

        return View(data);
    }

    [AuthorizePermission((int)Pages.Urgencies, (int)PageAccess.Read_Write)]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Urgencies, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Create(UrgencyCreateVM vm, CancellationToken ct = default) {
        if(!ModelState.IsValid)
            return View(vm);

        if (await _context.Urgencies.AnyAsync(x => x.Name.ToLower() == vm.Name.ToLower(), ct))
        {
            ModelState.AddModelError("Name", "This name already exists");
            return View(vm);
        }

        int Days = vm.Days ?? 0;

        int Hours = vm.Hours ?? 0;

        int Minutes = vm.Minutes ?? 0;

        Urgency urgency = new()
        {
            Name = vm.Name,
            Time = (Days * 24 * 60) + (Hours * 60) + Minutes,
        };

        await _context.Urgencies.AddAsync(urgency, ct);
        await _context.SaveChangesAsync(ct);
        
        return RedirectToAction(nameof(Index));
    }

    [AuthorizePermission((int)Pages.Urgencies, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(int id, CancellationToken ct = default)
    {
        Urgency urgency = await _getUrgencyAsync(id, ct);

        UrgencyUpdateVM vm = new()
        {
            Id = urgency.Id,
            Name = urgency.Name,
            Days = urgency.Time / (24 * 60),
            Hours = (urgency.Time - (urgency.Time / (24 * 60) * 24 * 60)) / 60,
            Minutes = urgency.Time % 60,
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Urgencies, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Update(UrgencyUpdateVM vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(vm);

        Urgency urgency = await _getUrgencyAsync(vm.Id, ct);

        if (urgency.Name != vm.Name)
        {
            if (await _context.Urgencies.AnyAsync(x => x.Name.ToLower() == vm.Name.ToLower(), ct))
            {
                ModelState.AddModelError("Name", "This name already exists");
                return View(vm);
            }
            urgency.Name = vm.Name;
        }

        int Days = vm.Days ?? 0;

        int Hours = vm.Hours ?? 0;

        int Minutes = vm.Minutes ?? 0;

        if (urgency.Time != (Days * 24 * 60) + (Hours * 60) + Minutes)
            urgency.Time = (Days * 24 * 60) + (Hours * 60) + Minutes;

        await _context.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission((int)Pages.Urgencies, (int)PageAccess.Read_Write)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        Urgency urgency = await _getUrgencyAsync(id, ct);

        _context.Urgencies.Remove(urgency);
        await _context.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Index));
    }

    public async Task<Urgency> _getUrgencyAsync(int id, CancellationToken ct = default)
    {
        return await _context.Urgencies.FindAsync(id, ct) ?? throw new Exception($"Urgency is not found with this id: {id}");
    }
}
