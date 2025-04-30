namespace TouristAgency.Models
{
    public class TravelPackage
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AvailableSlots { get; set; }

        public int DestinationId { get; set; }
        public Destination Destination { get; set; }

        public string TourOperatorId { get; set; }
        public ApplicationUser TourOperator { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }

}
