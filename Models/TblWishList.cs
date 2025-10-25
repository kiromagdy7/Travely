using System;
using System.Collections.Generic;

namespace Travely.Models;

public partial class TblWishList
{
    public int WishlistId { get; set; }

    public int UserId { get; set; }

    public int HotelId { get; set; }

    public DateTime AddedDate { get; set; }

    public virtual TblUser User { get; set; } = null!;

    // === ضيف السطر ده ===
    public virtual TblHotel Hotel { get; set; } = null!;
}