using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TouristAgency.Data;
using TouristAgency.Models;

public class PublicController : Controller
{
    private readonly ApplicationDbContext _context;

    public PublicController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return RedirectToAction("Destinations");
    }
    public async Task<IActionResult> Destinations(string search)
    {
        var query = _context.Destinations.Include(d => d.TravelPackages).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(d => d.Name.Contains(search));
        }

        var destinations = await query.ToListAsync();
        return View(destinations);
    }
}
