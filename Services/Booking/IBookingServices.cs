using System.Threading.Tasks;
using Travely.Dtos.Bookings;

namespace Travely.Services.Bookings
{
    public interface IBookingService
    {
        Task<BookingResult<BookingDto>> CreateAsync(CreateBookingDto dto);
        Task<BookingResult<BookingDto>> UpdateAsync(UpdateBookingDto dto);
        Task<BookingResult<BookingDto>> CancelAsync(int bookingId);
    }
}