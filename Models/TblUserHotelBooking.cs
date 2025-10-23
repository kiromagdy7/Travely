using System;
using System.Collections.Generic;

namespace Travely.Models;

public partial class TblUserHotelBooking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int HotelId { get; set; }

    public DateTime BookingDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual TblHotel Hotel { get; set; } = null!;

    public virtual TblReview? TblReview { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
