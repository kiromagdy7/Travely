using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Travely.Data;
using Travely.Dtos.Bookings;
using Travely.Models;

namespace Travely.Services.Bookings
{
    public class BookingService : IBookingService
    {
        private const string StatusConfirmed = "Confirmed";
        private const string StatusCancelled = "Cancelled";

        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResult<BookingDto>> CreateAsync(CreateBookingDto dto)
        {
            // Validate date range: CheckIn < CheckOut
            if (dto.CheckIn >= dto.CheckOut)
                return BookingResult<BookingDto>.Fail("Check-in date must be before the check-out date.", "INVALID_DATE_RANGE");

            // Validate CheckIn is not in the past (UTC date)
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            if (dto.CheckIn < today)
                return BookingResult<BookingDto>.Fail("Check-in date cannot be in the past.", "INVALID_CHECKIN_DATE");

            // Validate room existence
            var room = await _context.TblRooms.AsNoTracking().FirstOrDefaultAsync(r => r.RoomId == dto.RoomId);
            if (room == null)
                return BookingResult<BookingDto>.Fail("Selected room does not exist.", "ROOM_NOT_FOUND");

            // Availability: no overlapping bookings for same room where Status != "Cancelled"
            var hasConflict = await HasOverlappingBookingAsync(dto.RoomId, dto.CheckIn, dto.CheckOut, excludeBookingId: null);
            if (hasConflict)
                return BookingResult<BookingDto>.Fail("Room is not available for the selected dates.", "DATE_CONFLICT");

            // Calculate total price = price per night * nights
            var nights = (dto.CheckOut.ToDateTime(TimeOnly.MinValue) - dto.CheckIn.ToDateTime(TimeOnly.MinValue)).Days;
            if (nights <= 0)
                return BookingResult<BookingDto>.Fail("Booking must be at least one night.", "INVALID_DURATION");

            var booking = new TblBooking
            {
                UserId = dto.UserId,
                RoomId = dto.RoomId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                Adults = dto.Adults,
                Children = dto.Children,
                TotalPrice = room.Price * nights,
                BookingReference = await GenerateUniqueBookingReferenceAsync(),
                Status = StatusConfirmed
            };

            _context.TblBookings.Add(booking);
            await _context.SaveChangesAsync();

            return BookingResult<BookingDto>.Ok(MapToDto(booking), "Booking created and confirmed successfully.");
        }

        public async Task<BookingResult<BookingDto>> UpdateAsync(UpdateBookingDto dto)
        {
            var existing = await _context.TblBookings.FirstOrDefaultAsync(b => b.BookingId == dto.BookingId);
            if (existing == null)
                return BookingResult<BookingDto>.Fail("Booking not found.", "NOT_FOUND");

            // Validate date range: CheckIn < CheckOut
            if (dto.CheckIn >= dto.CheckOut)
                return BookingResult<BookingDto>.Fail("Check-in date must be before the check-out date.", "INVALID_DATE_RANGE");

            // Validate CheckIn is not in the past (UTC date)
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            if (dto.CheckIn < today)
                return BookingResult<BookingDto>.Fail("Check-in date cannot be in the past.", "INVALID_CHECKIN_DATE");

            // Validate room existence
            var room = await _context.TblRooms.AsNoTracking().FirstOrDefaultAsync(r => r.RoomId == dto.RoomId);
            if (room == null)
                return BookingResult<BookingDto>.Fail("Selected room does not exist.", "ROOM_NOT_FOUND");

            // Availability: exclude this booking from overlap check
            var hasConflict = await HasOverlappingBookingAsync(dto.RoomId, dto.CheckIn, dto.CheckOut, excludeBookingId: existing.BookingId);
            if (hasConflict)
                return BookingResult<BookingDto>.Fail("Room is not available for the selected dates.", "DATE_CONFLICT");

            // Recalculate total price
            var nights = (dto.CheckOut.ToDateTime(TimeOnly.MinValue) - dto.CheckIn.ToDateTime(TimeOnly.MinValue)).Days;
            if (nights <= 0)
                return BookingResult<BookingDto>.Fail("Booking must be at least one night.", "INVALID_DURATION");

            existing.UserId = dto.UserId;
            existing.RoomId = dto.RoomId;
            existing.CheckIn = dto.CheckIn;
            existing.CheckOut = dto.CheckOut;
            existing.Adults = dto.Adults;
            existing.Children = dto.Children;
            existing.TotalPrice = room.Price * nights;
            // Keep current status as-is during edit

            _context.TblBookings.Update(existing);
            await _context.SaveChangesAsync();

            return BookingResult<BookingDto>.Ok(MapToDto(existing), "Booking updated successfully.");
        }

        public async Task<BookingResult<BookingDto>> CancelAsync(int bookingId)
        {
            var booking = await _context.TblBookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
                return BookingResult<BookingDto>.Fail("Booking not found.", "NOT_FOUND");

            if (string.Equals(booking.Status, StatusCancelled, StringComparison.OrdinalIgnoreCase))
                return BookingResult<BookingDto>.Ok(MapToDto(booking), "Booking is already cancelled.");

            booking.Status = StatusCancelled;
            _context.TblBookings.Update(booking);
            await _context.SaveChangesAsync();

            return BookingResult<BookingDto>.Ok(MapToDto(booking), "Booking cancelled successfully.");
        }

        private async Task<bool> HasOverlappingBookingAsync(int roomId, DateOnly startDate, DateOnly endDate, int? excludeBookingId)
        {
            // Overlap when: startDate < existing.CheckOut AND existing.CheckIn < endDate
            return await _context.TblBookings
                .AsNoTracking()
                .Where(b => b.RoomId == roomId)
                .Where(b => !string.Equals(b.Status, StatusCancelled, StringComparison.OrdinalIgnoreCase))
                .Where(b => b.CheckIn.HasValue && b.CheckOut.HasValue)
                .Where(b => excludeBookingId == null || b.BookingId != excludeBookingId.Value)
                .AnyAsync(b => startDate < b.CheckOut!.Value && b.CheckIn!.Value < endDate);
        }

        private async Task<string> GenerateUniqueBookingReferenceAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                var reference = $"BK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}".Substring(0, 22);
                var exists = await _context.TblBookings.AsNoTracking().AnyAsync(b => b.BookingReference == reference);
                if (!exists)
                    return reference;
                await Task.Delay(5);
            }
            return $"BK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}".Substring(0, 26);
        }

        private static BookingDto MapToDto(TblBooking b) => new BookingDto
        {
            BookingId = b.BookingId,
            UserId = b.UserId,
            RoomId = b.RoomId,
            CheckIn = b.CheckIn,
            CheckOut = b.CheckOut,
            Status = b.Status,
            TotalPrice = b.TotalPrice,
            BookingReference = b.BookingReference,
            Adults = b.Adults,
            Children = b.Children
        };
    }
}