using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TouristAgency.Data;
using TouristAgency.Models;

namespace TouristAgency
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            string[] roles = { "Admin", "User", "TravelAgency" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Create users
            var admin = new ApplicationUser { UserName = "admin@demo.com", Email = "admin@demo.com", EmailConfirmed = true };
            var user = new ApplicationUser { UserName = "user@demo.com", Email = "user@demo.com", EmailConfirmed = true };
            var agency = new ApplicationUser { UserName = "agency@demo.com", Email = "agency@demo.com", EmailConfirmed = true, IsApproved = true };

            if (await userManager.FindByEmailAsync(admin.Email) == null)
            {
                await userManager.CreateAsync(admin, "Test1234!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (await userManager.FindByEmailAsync(user.Email) == null)
            {
                await userManager.CreateAsync(user, "Test1234!");
                await userManager.AddToRoleAsync(user, "User");
            }

            if (await userManager.FindByEmailAsync(agency.Email) == null)
            {
                await userManager.CreateAsync(agency, "Test1234!");
                await userManager.AddToRoleAsync(agency, "TravelAgency");
            }

            if (!context.Destinations.Any())
            {
                var dest1 = new Destination { Name = "Малдиви", Description = "Екзотика", Latitude = 3.2, Longitude = 73.2 };
                var dest2 = new Destination { Name = "Исландия", Description = "Северна красота", Latitude = 65.0, Longitude = -18.0 };
                var dest3 = new Destination { Name = "Мароко", Description = "Африканска приказка", Latitude = 31.0, Longitude = -7.0 };

                context.Destinations.AddRange(dest1, dest2, dest3);
                await context.SaveChangesAsync();

                var destinations = context.Destinations.ToList();
                string agencyId = (await userManager.FindByEmailAsync("agency@demo.com")).Id;

                var pkg1 = new TravelPackage
                {
                    Title = "Малдивско приключение",
                    Description = "7 дни на райски плажове",
                    Price = 2999,
                    StartDate = DateTime.Today.AddDays(30),
                    EndDate = DateTime.Today.AddDays(37),
                    AvailableSlots = 10,
                    DestinationId = destinations[0].Id,
                    TourOperatorId = agencyId
                };
                var pkg2 = new TravelPackage
                {
                    Title = "Исландски чудеса",
                    Description = "Открий северното сияние",
                    Price = 2499,
                    StartDate = DateTime.Today.AddDays(45),
                    EndDate = DateTime.Today.AddDays(52),
                    AvailableSlots = 8,
                    DestinationId = destinations[1].Id,
                    TourOperatorId = agencyId
                };
                var pkg3 = new TravelPackage
                {
                    Title = "Мароко: Сахара и пазари",
                    Description = "Екзотично пътуване през пустинята",
                    Price = 1999,
                    StartDate = DateTime.Today.AddDays(60),
                    EndDate = DateTime.Today.AddDays(67),
                    AvailableSlots = 12,
                    DestinationId = destinations[2].Id,
                    TourOperatorId = agencyId
                };

                context.TravelPackages.AddRange(pkg1, pkg2, pkg3);
                await context.SaveChangesAsync();

                string uploads = Path.Combine("wwwroot", "uploads", "packages");
                Directory.CreateDirectory(uploads);

                var img1 = new TravelPackageImage { TravelPackageId = pkg1.Id, ImagePath = "maldives.jpg" };
                var img2 = new TravelPackageImage { TravelPackageId = pkg2.Id, ImagePath = "iceland.jpg" };
                var img3 = new TravelPackageImage { TravelPackageId = pkg3.Id, ImagePath = "morocco.jpg" };

                context.TravelPackageImages.AddRange(img1, img2, img3);

                context.DestinationImages.AddRange(
                    new DestinationImage { DestinationId = destinations[0].Id, FileName = "maldives.jpg" },
                    new DestinationImage { DestinationId = destinations[1].Id, FileName = "iceland.jpg" },
                    new DestinationImage { DestinationId = destinations[2].Id, FileName = "morocco.jpg" }
                );

                await context.SaveChangesAsync();

                // Booking
                context.Bookings.Add(new Booking
                {
                    TravelPackageId = pkg1.Id,
                    UserId = (await userManager.FindByEmailAsync("user@demo.com")).Id,
                    Status = "Изчакваща",
                    CreatedOn = DateTime.Now,
                    NumberOfSlots = 1,
                    TotalPrice = pkg1.Price,
                    Passengers = new List<BookingPassenger> {
                    new BookingPassenger { FullName = "Иван Иванов" }
                }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
