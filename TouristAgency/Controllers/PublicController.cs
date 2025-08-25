using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TouristAgency.Data;
using TouristAgency.Models;

public class PublicController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PublicController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var destinations = await _context.Destinations.Include(d => d.Images).ToListAsync();
        return View("Destinations", destinations);
    }

    public async Task<IActionResult> Destinations(string search)
    {
         var query = _context.Destinations
        .Include(d => d.Images)  
        .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(d => d.Name.Contains(search));
        }

        var destinations = await query.ToListAsync();
        return View(destinations);
    }

    public async Task<IActionResult> Destination(int id, DateTime? startDate, DateTime? endDate)
    {
    var destination = await _context.Destinations
        .Include(d => d.Images)
        .Include(d => d.TravelPackages)
            .ThenInclude(tp => tp.Images)
        .FirstOrDefaultAsync(d => d.Id == id);

    if (destination == null) return NotFound();

    var packages = destination.TravelPackages.AsQueryable();

    if (startDate.HasValue)
        packages = packages.Where(p => p.StartDate >= startDate.Value);

    if (endDate.HasValue)
        packages = packages.Where(p => p.EndDate <= endDate.Value);

    ViewBag.FilteredPackages = packages.ToList();
    ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
    ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

    return View(destination);
}


    public async Task<IActionResult> Agency(string id)
    {
        var agency = await _context.Users
            .Include(u => u.TravelPackages)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (agency == null) return NotFound();

        var isTravelAgency = await _userManager.IsInRoleAsync(agency, "TravelAgency");
        if (!isTravelAgency) return Forbid();

        return View(agency);
    }
}
