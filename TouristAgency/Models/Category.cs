namespace TouristAgency.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Destination> Destinations { get; set; }
    }
}
