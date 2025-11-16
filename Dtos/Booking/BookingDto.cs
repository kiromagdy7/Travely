using System;

namespace Travely.Dtos.Bookings
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateOnly? CheckIn { get; set; }
        public DateOnly? CheckOut { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public int Adults { get; set; }
        public int? Children { get; set; }
    }
}