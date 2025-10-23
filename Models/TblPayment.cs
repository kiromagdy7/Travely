using System;
using System.Collections.Generic;

namespace Travely.Models;

public partial class TblPayment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public int UserId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public virtual TblBooking Booking { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
