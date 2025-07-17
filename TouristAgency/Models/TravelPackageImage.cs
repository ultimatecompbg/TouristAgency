public class TravelPackageImage
{
    public int Id { get; set; }
    public string ImagePath { get; set; }

    public int TravelPackageId { get; set; }
    public TravelPackage TravelPackage { get; set; }
}
