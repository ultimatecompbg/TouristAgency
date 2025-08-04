using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TouristAgency.Models
{
    public class TravelPackage : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, 1000)]
        public int AvailableSlots { get; set; }

        [Required]
        public int DestinationId { get; set; }

        public Destination? Destination { get; set; }
        public string? ImagePath { get; set; }

        public string? TourOperatorId { get; set; }

        public ApplicationUser? TourOperator { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
        [NotMapped]
        public List<IFormFile> ImageFiles { get; set; }
        public ICollection<TravelPackageImage> Images { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.Date < DateTime.Today)
            {
                yield return new ValidationResult("Началната дата не може да е в миналото.", new[] { nameof(StartDate) });
            }

            if (EndDate.Date < StartDate.Date)
            {
                yield return new ValidationResult("Крайната дата не може да е преди началната.", new[] { nameof(EndDate) });
            }
        }

    }
}
