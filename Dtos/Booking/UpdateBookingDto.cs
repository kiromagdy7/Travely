using System;

namespace Travely.Dtos.Bookings
{
    public class UpdateBookingDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int Adults { get; set; }
        public int? Children { get; set; }
    }
}