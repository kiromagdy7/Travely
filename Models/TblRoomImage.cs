using System;
using System.Collections.Generic;

namespace Travely.Models;

public partial class TblRoomImage
{
    public int ImageId { get; set; }

    public int RoomId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual TblRoom Room { get; set; } = null!;
}
