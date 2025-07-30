using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TouristAgency.Data;
using TouristAgency.Models;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // User management
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
        var agencies = new List<ApplicationUser>();

        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(user, "TravelAgency"))
            {
                agencies.Add(user);
            }
        }

        return View(agencies);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && await _userManager.IsInRoleAsync(user, "TravelAgency"))
        {
            user.IsApproved = true;
            await _userManager.UpdateAsync(user);
        }
        return RedirectToAction("Users");
    }

    [HttpPost]
    public async Task<IActionResult> Revoke(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && await _userManager.IsInRoleAsync(user, "TravelAgency"))
        {
            user.IsApproved = false;
            await _userManager.UpdateAsync(user);
        }
        return RedirectToAction("Users");
    }

    [HttpGet]
    public async Task<IActionResult> ViewAgency(string id)
    {
        var agency = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (agency == null || !(await _userManager.IsInRoleAsync(agency, "TravelAgency"))) return NotFound();
        return View("~/Views/Admin/ViewAgency.cshtml", agency);
    }

    // Destination management
    public async Task<IActionResult> Destinations()
    {
        var destinations = await _context.Destinations.ToListAsync();
        return View(destinations);
    }

    [HttpGet]
    public IActionResult CreateDestination()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDestination(Destination destination)
    {
        if (!ModelState.IsValid)
            return View(destination);

        _context.Add(destination);
        await _context.SaveChangesAsync();

        if (destination.ImageFiles != null && destination.ImageFiles.Count > 0)
        {
            var uploadDir = Path.Combine("wwwroot", "uploads", "destinations");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            foreach (var file in destination.ImageFiles)
            {
                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadDir, uniqueName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }

                _context.DestinationImages.Add(new DestinationImage
                {
                    DestinationId = destination.Id,
                    FileName = uniqueName
                });
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Destinations));
    }




    [HttpGet]
    public async Task<IActionResult> EditDestination(int id)
    {
        var destination = await _context.Destinations
            .Include(d => d.Images)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (destination == null) return NotFound();

        return View(destination);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDestination(int id, Destination destination, List<IFormFile> NewImages, List<int> DeleteImageIds)
    {
        if (id != destination.Id)
            return NotFound();

        var existingDestination = await _context.Destinations
            .Include(d => d.Images)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (existingDestination == null)
            return NotFound();

        // Update fields
        existingDestination.Name = destination.Name;
        existingDestination.Description = destination.Description;
        existingDestination.Latitude = destination.Latitude;
        existingDestination.Longitude = destination.Longitude;

        // Delete selected images
        if (DeleteImageIds != null)
        {
            foreach (var imageId in DeleteImageIds)
            {
                var image = await _context.DestinationImages.FindAsync(imageId);
                if (image != null)
                {
                    var filePath = Path.Combine("wwwroot", "uploads", "destinations", image.FileName);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);

                    _context.DestinationImages.Remove(image);
                }
            }
        }

        // Add new images
        if (NewImages != null && NewImages.Count > 0)
        {
            var uploadDir = Path.Combine("wwwroot", "uploads", "destinations");
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

                _context.DestinationImages.Add(new DestinationImage
                {
                    DestinationId = destination.Id,
                    FileName = uniqueName
                });
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Destinations));
    }


    [HttpGet]
    public async Task<IActionResult> DeleteDestination(int id)
    {
        var destination = await _context.Destinations.FindAsync(id);
        if (destination == null) return NotFound();
        return View(destination);   
    }
    [HttpPost]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var image = await _context.DestinationImages.FindAsync(imageId);
        if (image != null)
        {
            var filePath = Path.Combine("wwwroot", "uploads", "destinations", image.FileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _context.DestinationImages.Remove(image);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("EditDestination", new { id = image?.DestinationId });
    }

    [HttpPost, ActionName("DeleteDestination")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDestinationConfirmed(int id)
    {
        var destination = await _context.Destinations.FindAsync(id);
        if (destination != null)
        {
            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Destinations));
    }
}
