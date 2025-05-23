using TouristAgency.Models;

namespace TouristAgency.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int TravelPackageId { get; set; }
        public TravelPackage TravelPackage { get; set; }

        public int NumberOfSlots { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public ICollection<BookingPassenger> Passengers { get; set; }
    }
}