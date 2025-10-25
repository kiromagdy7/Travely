using System;

namespace Travely.Dtos.Bookings
{
    public class CreateBookingDto
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public byte Adults { get; set; } = 1;
        public byte? Children { get; set; } = 0;
    }
}