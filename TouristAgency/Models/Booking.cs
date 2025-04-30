namespace TouristAgency.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int TravelPackageId { get; set; }
        public TravelPackage TravelPackage { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }

}
