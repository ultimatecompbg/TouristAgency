

using Microsoft.AspNetCore.Identity;

namespace TouristAgency.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; } = false;
        public ICollection<TravelPackage>? TravelPackages { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
