

using Microsoft.AspNetCore.Identity;

namespace TouristAgency.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; } = false;
    }
}
