using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TouristAgency.Models;

namespace TouristAgency.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TravelPackage> TravelPackages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
