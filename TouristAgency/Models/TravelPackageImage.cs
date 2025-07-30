namespace TouristAgency.Models;

public class TravelPackageImage
{
    public int Id { get; set; }

    public int TravelPackageId { get; set; }
    public TravelPackage TravelPackage { get; set; }

    public string FileName { get; set; } 
}
