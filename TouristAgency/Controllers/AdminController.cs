using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        var users = await _context.Users.ToListAsync();
        return View(users);
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
        if (ModelState.IsValid)
        {
            _context.Add(destination);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Destinations));
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
