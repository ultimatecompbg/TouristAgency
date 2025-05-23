using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TouristAgency.Models
{
    public class BookingViewModel
    {
        public int TravelPackageId { get; set; }

        [Range(1, 10, ErrorMessage = "Между 1 и 10.")]
        public int NumberOfSlots { get; set; }

        public List<string> PassengerNames { get; set; }
    }
}
