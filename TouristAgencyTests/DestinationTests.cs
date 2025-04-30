using NUnit.Framework;
using TouristAgency.Models;

namespace TouristAgency.Tests
{
    public class DestinationTests
    {
        [Test]
        public void Destination_WithMinimalValidData_ShouldCreateSuccessfully()
        {
            var d = new Destination
            {
                Name = "Test",
                Latitude = 42.0,
                Longitude = 25.0,
                Description = "Test description"
            };

            Assert.That(d.Name, Is.EqualTo("Test"));
        }

        [Test]
        public void Latitude_ShouldAcceptPositiveNumber()
        {
            var d = new Destination { Latitude = 43.123456 };
            Assert.That(d.Latitude, Is.EqualTo(43.123456));
        }

        [Test]
        public void OptionalFields_CanBeNull()
        {
            var d = new Destination
            {
                Name = "EmptyTest",
                Latitude = 0,
                Longitude = 0,
                Description = "Nothing"
            };

            Assert.That(d.Season, Is.Null);
            Assert.That(d.ImageUrl, Is.Null);
        }
    }
}
