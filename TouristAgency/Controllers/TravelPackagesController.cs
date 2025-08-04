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
public async Task<IActionResult> Create(TravelPackage package)
{
    
    ModelState.Remove("Images");
    if (!ModelState.IsValid)
        {
            ViewBag.Destinations = new SelectList(_context.Destinations, "Id", "Name", package.DestinationId);
            return View(package);
        }
    package.TourOperatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);    
    _context.TravelPackages.Add(package);
    await _context.SaveChangesAsync();

    if (package.ImageFiles != null && package.ImageFiles.Any())
    {
        var uploadDir = Path.Combine("wwwroot", "uploads", "packages");
        if (!Directory.Exists(uploadDir))
            Directory.CreateDirectory(uploadDir);

        foreach (var file in package.ImageFiles)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _context.TravelPackageImages.Add(new TravelPackageImage
            {
                TravelPackageId = package.Id,
                ImagePath = fileName
            });
        }

        await _context.SaveChangesAsync();
    }

    return RedirectToAction(nameof(MyPackages));
}


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var package = await _context.TravelPackages
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package == null) return NotFound();

        ViewBag.Destinations = new SelectList(_context.Destinations, "Id", "Name", package.DestinationId);
        return View(package);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TravelPackage package, List<IFormFile> NewImages, List<int> DeleteImageIds)
    {
        if (id != package.Id) return NotFound();

        var existingPackage = await _context.TravelPackages
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existingPackage == null) return NotFound();

        existingPackage.Title = package.Title;
        existingPackage.Description = package.Description;
        existingPackage.Price = package.Price;
        existingPackage.StartDate = package.StartDate;
        existingPackage.EndDate = package.EndDate;
        existingPackage.AvailableSlots = package.AvailableSlots;
        existingPackage.DestinationId = package.DestinationId;

        if (DeleteImageIds != null)
        {
            foreach (var imageId in DeleteImageIds)
            {
                var image = await _context.TravelPackageImages.FindAsync(imageId);
                if (image != null)
                {
                    var filePath = Path.Combine("wwwroot", "uploads", "packages", image.ImagePath);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);

                    _context.TravelPackageImages.Remove(image);
                }
            }
        }

        if (NewImages != null && NewImages.Count > 0)
        {
            var uploadDir = Path.Combine("wwwroot", "uploads", "packages");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            foreach (var file in NewImages)
            {
                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadDir, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _context.TravelPackageImages.Add(new TravelPackageImage
                {
                    TravelPackageId = package.Id,
                    ImagePath = uniqueName
                });
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(MyPackages));
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
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package == null)
            return NotFound();

        return View(package);
    }
}
