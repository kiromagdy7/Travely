using System;
using System.Collections.Generic;

namespace Travely.Models;

public partial class LkpAmenity
{
    public int AmenityId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TblHotel> Hotels { get; set; } = new List<TblHotel>();
}
