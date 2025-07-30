namespace TouristAgency.Models
{
    public class DestinationImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        public int DestinationId { get; set; }
        public Destination Destination { get; set; }
    }
}
