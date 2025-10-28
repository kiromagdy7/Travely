// داخل مجلد ViewModels
public class BookingInfoViewModel
{
    public int BookingId { get; set; }
    public string HotelName { get; set; }
    public string Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
}