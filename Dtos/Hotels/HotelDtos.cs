using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Travely.Dtos.Hotels
{
    public class HotelImageDto
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class HotelRoomDto
    {
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public string? RoomType { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public byte? BedsCount { get; set; }
        public byte? MaxGuests { get; set; }
    }

    public class HotelDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte? Stars { get; set; }
        public string? ContactInfo { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string? CancellationPolicy { get; set; }
        public string? Fees { get; set; }
        public decimal? Commission { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Overview { get; set; }
        public string? Description { get; set; }

        public List<HotelImageDto> Images { get; set; } = new();
        public List<HotelRoomDto> Rooms { get; set; } = new();
    }

    public class CreateHotelDto
    {
        [Required, StringLength(250)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public byte? Stars { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? ContactInfo { get; set; }

        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }

        public string? CancellationPolicy { get; set; }

        [StringLength(500)]
        public string? Fees { get; set; }

        [Range(typeof(decimal), "0", "999.99")]
        public decimal? Commission { get; set; }
        public string? Overview { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateHotelDto : CreateHotelDto
    {
        [Required]
        public int HotelId { get; set; }
    }
}