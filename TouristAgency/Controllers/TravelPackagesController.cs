using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using TouristAgency.Data;
using TouristAgency.Models;

[Authorize(Roles = "TravelAgency")]
public class TravelPackagesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TravelPackagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private async Task<bool> IsUserApproved()
    {
        var user = await _userManager.GetUserAsync(User);
        return user != null && user.IsApproved;
    }

    public async Task<IActionResult> MyPackages()
    {
        if (!await IsUserApproved()) return Forbid();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var packages = await _context.TravelPackages
            .Include(p => p.Destination)
            .Where(p => p.TourOperatorId == userId)
            .ToListAsync();
        return View(packages);
    }

    public async Task<IActionResult> Create()
    {
        if (!await IsUserApproved()) return Forbid();

        ViewData["Destinations"] = new SelectList(_context.Destinations, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TravelPackage model)
    {
        if (!await IsUserApproved()) return Forbid();

        if (ModelState.IsValid)
        {
            model.TourOperatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyPackages));
        }
        else
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Debug.WriteLine($"❌ {entry.Key}: {error.ErrorMessage}");
                    }
                }
            }

        }
        ViewData["Destinations"] = new SelectList(_context.Destinations, "Id", "Name", model.DestinationId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!await IsUserApproved()) return Forbid();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var package = await _context.TravelPackages.FindAsync(id);
        if (package == null || package.TourOperatorId != userId)
        {
            return NotFound();
        }
        ViewData["Destinations"] = new SelectList(_context.Destinations, "Id", "Name", package.DestinationId);
        return View(package);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TravelPackage model)
    {
        if (!await IsUserApproved()) return Forbid();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var original = await _context.TravelPackages.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        if (original == null || original.TourOperatorId != userId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            model.TourOperatorId = userId;
            _context.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyPackages));
        }
        else {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Debug.WriteLine($"❌ {entry.Key}: {error.ErrorMessage}");
                    }
                }
            }

        }
        ViewData["Destinations"] = new SelectList(_context.Destinations, "Id", "Name", model.DestinationId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await IsUserApproved()) return Forbid();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var package = await _context.TravelPackages
            .Include(p => p.Destination)
            .FirstOrDefaultAsync(p => p.Id == id && p.TourOperatorId == userId);

        if (package == null)
        {
            return NotFound();
        }

        return View(package);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await IsUserApproved()) return Forbid();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var package = await _context.TravelPackages.FindAsync(id);
        if (package == null || package.TourOperatorId != userId)
        {
            return NotFound();
        }

        _context.TravelPackages.Remove(package);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(MyPackages));
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var package = await _context.TravelPackages
            .Include(p => p.Destination)
            .Include(p => p.TourOperator)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package == null)
            return NotFound();

        return View(package);
    }


}
