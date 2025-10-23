using System;
using System.Collections.Generic;

namespace Travely.Models;

public partial class TblUser
{
    public int UserId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Phone { get; set; }

    public byte? Age { get; set; }

    public string Role { get; set; } = null!;

    public string? Imagepath { get; set; }

    public string? Country { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TblBooking> TblBookings { get; set; } = new List<TblBooking>();

    public virtual ICollection<TblPayment> TblPayments { get; set; } = new List<TblPayment>();

    public virtual ICollection<TblUserHotelBooking> TblUserHotelBookings { get; set; } = new List<TblUserHotelBooking>();

    public virtual ICollection<TblWishList> TblWishLists { get; set; } = new List<TblWishList>();
}
