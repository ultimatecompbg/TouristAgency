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
    Debug.WriteLine($"Получена дестинация: {destination.Name}, lat: {destination.Latitude}, lng: {destination.Longitude}");

    if (ModelState.IsValid)
    {
            // ✅ Handle image upload
            if (destination.ImageFile != null && destination.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(destination.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await destination.ImageFile.CopyToAsync(stream);
                }

                destination.ImagePath = "/images/" + uniqueFileName;
                Debug.WriteLine("Posting!");
        }

        _context.Add(destination);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Destinations));
    }
    else
    {
        foreach (var entry in ModelState)
        {
            foreach (var error in entry.Value.Errors)
            {
                Debug.WriteLine($"ГРЕШКА при '{entry.Key}': {error.ErrorMessage}");
            }
        }
    }

    return View(destination);
}


    [HttpGet]
    public async Task<IActionResult> EditDestination(int id)
    {
        var destination = await _context.Destinations.FindAsync(id);
        if (destination == null) return NotFound();
        return View(destination);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDestination(int id, Destination destination)
    {
        if (id != destination.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(destination);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Destinations));
        }
        return View(destination);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteDestination(int id)
    {
        var destination = await _context.Destinations.FindAsync(id);
        if (destination == null) return NotFound();
        return View(destination);   
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
