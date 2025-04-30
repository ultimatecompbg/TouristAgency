using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

    public async Task<IActionResult> MyPackages()
    {
        var userId = _userManager.GetUserId(User);
        var packages = await _context.TravelPackages
            .Include(p => p.Destination)
            .Where(p => p.TourOperatorId == userId)
            .ToListAsync();
        return View(packages);
    }

    public IActionResult Create()
    {
        ViewData["Destinations"] = new SelectList(_context.Destinations, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TravelPackage model)
    {
        if (ModelState.IsValid)
        {
            model.TourOperatorId = _userManager.GetUserId(User);
            _context.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyPackages));
        }
        ViewData["Destinations"] = new SelectList(_context.Destinations, "Id", "Name", model.DestinationId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
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
        var userId = _userManager.GetUserId(User);
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
        ViewData["Destinations"] = new SelectList(_context.Destinations, "Id", "Name", model.DestinationId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
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
        var userId = _userManager.GetUserId(User);
        var package = await _context.TravelPackages.FindAsync(id);
        if (package == null || package.TourOperatorId != userId)
        {
            return NotFound();
        }

        _context.TravelPackages.Remove(package);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(MyPackages));
    }
}
