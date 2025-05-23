using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TouristAgency.Data;
using TouristAgency.Models;

namespace TouristAgency.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ADMIN & AGENCY VIEW
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bookings
                .Include(b => b.TravelPackage)
                .Include(b => b.User);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.TravelPackage)
                    .ThenInclude(p => p.Destination)
                .Include(b => b.TravelPackage)
                    .ThenInclude(p => p.TourOperator)
                .Include(b => b.Passengers)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }


        public IActionResult Create()
        {
            ViewData["TravelPackageId"] = new SelectList(_context.TravelPackages, "Id", "Title");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,TravelPackageId,Status,CreatedOn")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Confirmation), new { id = booking.Id });
            }
            ViewData["TravelPackageId"] = new SelectList(_context.TravelPackages, "Id", "Title", booking.TravelPackageId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
            return View(booking);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewData["TravelPackageId"] = new SelectList(_context.TravelPackages, "Id", "Title", booking.TravelPackageId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,TravelPackageId,Status,CreatedOn")] Booking booking)
        {
            if (id != booking.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bookings.Any(e => e.Id == booking.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TravelPackageId"] = new SelectList(_context.TravelPackages, "Id", "Title", booking.TravelPackageId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", booking.UserId);
            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.TravelPackage)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // USER FLOW
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> CreateForUser(int id)
        {
            var travelPackage = await _context.TravelPackages.FindAsync(id);
            if (travelPackage == null) return NotFound();

            ViewBag.Package = travelPackage;
            return View("UserCreate", new BookingViewModel { TravelPackageId = id });
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForUser(BookingViewModel model)
        {
            var travelPackage = await _context.TravelPackages.FindAsync(model.TravelPackageId);
            if (travelPackage == null) return NotFound();

            if (!ModelState.IsValid || model.PassengerNames == null || model.PassengerNames.Count != model.NumberOfSlots)
            {
                ViewBag.Package = travelPackage;
                ModelState.AddModelError("", "Моля, въведете име за всяко място.");
                return View("UserCreate", model);
            }

            var booking = new Booking
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                TravelPackageId = model.TravelPackageId,
                NumberOfSlots = model.NumberOfSlots,
                TotalPrice = travelPackage.Price * model.NumberOfSlots,
                Status = "Paid",
                CreatedOn = DateTime.UtcNow,
                Passengers = model.PassengerNames.Select(name => new BookingPassenger { FullName = name }).ToList()
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmation), new { id = booking.Id });
        }
       

        [Authorize]
        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.TravelPackage)
                .Include(b => b.Passengers)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (booking == null) return NotFound();

            return View("Confirmation", booking);
        }

        [Authorize]
        public async Task<IActionResult> Invoice(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.TravelPackage)
                .Include(b => b.Passengers)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (booking == null) return NotFound();

            return View("Invoice", booking);
        }
        [Authorize(Roles = "TravelAgency")]
        public async Task<IActionResult> AgencyReservations()
        {
            var agencyId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookings = await _context.Bookings
                .Include(b => b.TravelPackage)
                    .ThenInclude(tp => tp.Destination)
                .Include(b => b.Passengers)
                .Include(b => b.User)
                .Where(b => b.TravelPackage.TourOperatorId == agencyId)
                .ToListAsync();

            return View("AgencyReservations", bookings);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _context.Bookings
                .Include(b => b.TravelPackage)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return View("MyBookings", bookings);
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromPackage(int packageId, int slotCount, List<string> travelerNames)
        {
            var user = await _userManager.GetUserAsync(User);
            var package = await _context.TravelPackages.FindAsync(packageId);

            if (package == null || slotCount < 1 || travelerNames.Count != slotCount)
                return BadRequest();

            if (package.AvailableSlots < slotCount)
            {
                TempData["Error"] = "Няма достатъчно свободни места.";
                return RedirectToAction("Details", "TravelPackages", new { id = packageId });
            }

            var booking = new Booking
            {
                TravelPackageId = packageId,
                UserId = user.Id,
                Status = "Изчакваща",
                CreatedOn = DateTime.Now,
                NumberOfSlots = slotCount,
                TotalPrice = package.Price * slotCount,
                Passengers = travelerNames.Select(name => new BookingPassenger
                {
                    FullName = name
                }).ToList()
            };

            package.AvailableSlots -= slotCount;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // ✅ Redirect to payment/confirmation page
            return RedirectToAction("ConfirmBooking", new { id = booking.Id });
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.TravelPackage)
                    .ThenInclude(tp => tp.Destination)
                .Include(b => b.TravelPackage)
                    .ThenInclude(tp => tp.TourOperator)
                .Include(b => b.Passengers)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            booking.Status = "Платена";
            if (booking == null)
                return NotFound();

            return View("ConfirmBooking", booking);
        }



    }
}
