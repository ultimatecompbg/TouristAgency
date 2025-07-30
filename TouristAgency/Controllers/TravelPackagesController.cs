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

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Destinations = new SelectList(_context.Destinations, "Id", "Name");
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TravelPackage package, List<IFormFile> ImageFiles)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Destinations = new SelectList(_context.Destinations, "Id", "Name", package.DestinationId);
            return View(package);
        }

        // Запази пакета
        _context.TravelPackages.Add(package);
        await _context.SaveChangesAsync();

        // Запази снимките
        if (ImageFiles != null && ImageFiles.Count > 0)
        {
            var uploadDir = Path.Combine("wwwroot", "uploads", "packages");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            foreach (var file in ImageFiles)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var path = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var travelImage = new TravelPackageImage
                {
                    TravelPackageId = package.Id,
                    FileName = fileName
                };

                _context.TravelPackageImages.Add(travelImage);
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
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
        var package = await _context.TravelPackages
            .Include(p => p.Bookings)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package == null)
            return NotFound();

        if (package.Bookings != null && package.Bookings.Any())
        {
            TempData["Error"] = "Не можете да изтриете пакет, за който има направени резервации.";
            return RedirectToAction(nameof(MyPackages));
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
