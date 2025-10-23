using System;
using System.Collections.Generic;

namespace Travely.Models;

public partial class TblHotelImage
{
    public int ImageId { get; set; }

    public int HotelId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual TblHotel Hotel { get; set; } = null!;
}
