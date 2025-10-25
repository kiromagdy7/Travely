using System;
using System.Collections.Generic;

namespace Travely.Models;

public partial class TblRoom
{
    public int RoomId { get; set; }

    public int HotelId { get; set; }

    public string? RoomNumber { get; set; }

    public string? RoomType { get; set; }

    public byte? BedsCount { get; set; }

    public decimal Price { get; set; }

    public byte? MaxGuests { get; set; }

    public string? Description { get; set; }

    public bool BreakfastIncluded { get; set; }

    public bool Available { get; set; }

    public bool PetsAllowed { get; set; }

    public DateTime CreatedAt { get; set; }

    // I REMOVED THE DUPLICATE "TblHotel" PROPERTY THAT WAS HERE.

    public virtual ICollection<TblBooking> TblBookings { get; set; } = new List<TblBooking>();

    public virtual ICollection<TblRoomImage> TblRoomImages { get; set; } = new List<TblRoomImage>();

    // This is the correct navigation property that links to "HotelId"
    public virtual TblHotel Hotel { get; set; } = null!;
}