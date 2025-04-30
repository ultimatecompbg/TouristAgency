using NUnit.Framework;
using TouristAgency.Models;
using System;

namespace TouristAgency.Tests
{
    public class BookingTests
    {
        [Test]
        public void Booking_DefaultStatus_IsNull()
        {
            var booking = new Booking();
            Assert.That(booking.Status, Is.Null);
        }

        [Test]
        public void Booking_CreatedOn_Today()
        {
            var b = new Booking { CreatedOn = DateTime.Today };
            Assert.That(b.CreatedOn.Date, Is.EqualTo(DateTime.Today));
        }
    }
}
