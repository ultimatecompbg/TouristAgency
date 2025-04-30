using NUnit.Framework;
using TouristAgency.Models;

namespace TouristAgency.Tests
{
    public class TravelPackageTests
    {
        [Test]
        public void CanCreateTravelPackageWithBasicInfo()
        {
            var p = new TravelPackage
            {
                Title = "Beach Escape",
                Price = 199.99m
            };

            Assert.Multiple(() =>
            {
                Assert.That(p.Title, Is.EqualTo("Beach Escape"));
                Assert.That(p.Price, Is.GreaterThan(0));
            });
        }
    }
}
