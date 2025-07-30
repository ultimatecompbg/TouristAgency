using System.ComponentModel.DataAnnotations.Schema;

namespace TouristAgency.Models
{
    public class Destination
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public string? Season { get; set; }
        public string? FunFacts { get; set; }
        public string? History { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<TravelPackage>? TravelPackages { get; set; }

        public ICollection<DestinationImage>? Images { get; set; }

        [NotMapped]
        public List<IFormFile>? ImageFiles { get; set; } 
    }
}
