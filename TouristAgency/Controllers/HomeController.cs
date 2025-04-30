using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TouristAgency.Data;

namespace TouristAgency.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var packages = await _context.TravelPackages
                .Include(p => p.Destination)
                .OrderByDescending(p => p.Id)
                .Take(3)
                .ToListAsync();

            return View(packages);
        }
    }
}

