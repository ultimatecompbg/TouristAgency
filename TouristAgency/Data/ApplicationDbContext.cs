using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TouristAgency.Models;

namespace TouristAgency.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // 🔧 This empty constructor helps Visual Studio scaffold properly
        public ApplicationDbContext() { }

        // ✅ This is the real constructor used at runtime
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // 💡 This makes sure scaffold has a DB provider even when DI isn't available
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TouristAgency;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TravelPackage> TravelPackages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
