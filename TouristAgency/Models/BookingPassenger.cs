using System.ComponentModel.DataAnnotations;

namespace TouristAgency.Models
{
    public class BookingPassenger
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        [Required]
        public string FullName { get; set; }
    }

}
