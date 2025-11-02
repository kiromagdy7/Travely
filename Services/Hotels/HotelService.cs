using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Travely.Data;
using Travely.Dtos.Hotels;
using Travely.Models;
using Travely.Services.Storage;

namespace Travely.Services.Hotels
{
    public class HotelService : IHotelService
    {
        private readonly AppDbContext _context;
        private readonly IImageStorage _imageStorage;

        public HotelService(AppDbContext context, IImageStorage imageStorage)
        {
            _context = context;
            _imageStorage = imageStorage;
        }

        public async Task<List<HotelDto>> GetAllAsync(bool includeRooms = false)
        {
            // IMPORTANT: Keep this as an entity query until Includes are applied
            IQueryable<TblHotel> query = _context.TblHotels
                .AsNoTracking()
                .Include(h => h.TblHotelImages);

            if (includeRooms)
            {
                // This Include compiles because query is IQueryable<TblHotel> (entity type)
                query = query.Include(h => h.TblRooms);
            }

            var hotels = await query.OrderBy(h => h.Name).ToListAsync();

            return hotels.Select(MapToDto).ToList();
        }

        public async Task<HotelDto?> GetByIdAsync(int hotelId, bool includeRooms = true)
        {
            IQueryable<TblHotel> query = _context.TblHotels
                .AsNoTracking()
                .Include(h => h.TblHotelImages);

            if (includeRooms)
            {
                query = query.Include(h => h.TblRooms);
            }

            var hotel = await query.FirstOrDefaultAsync(h => h.HotelId == hotelId);
            return hotel is null ? null : MapToDto(hotel);
        }

        public async Task<(bool Success, string Message, int? HotelId)> CreateAsync(CreateHotelDto dto, IEnumerable<IFormFile>? images)
        {
            // Uniqueness check for Name (matches Db unique index)
            var nameExists = await _context.TblHotels.AsNoTracking().AnyAsync(h => h.Name == dto.Name);
            if (nameExists)
                return (false, "This hotel name is already in use.", null);

            if (dto.Stars is > 5)
                return (false, "Stars must be between 0 and 5.", null);

            var entity = new TblHotel
            {
                Name = dto.Name,
                Location = dto.Location,
                Address = dto.Address,
                Stars = dto.Stars,
                Phone = dto.Phone,
                ContactInfo = dto.ContactInfo,
                CheckInTime = dto.CheckInTime,
                CheckOutTime = dto.CheckOutTime,
                Overview = dto.Overview,
                Description = dto.Description,
                CancellationPolicy = dto.CancellationPolicy,
                Fees = dto.Fees,
                Commission = dto.Commission ?? 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.TblHotels.Add(entity);
            await _context.SaveChangesAsync();

            if (images != null)
            {
                foreach (var img in images.Where(f => f?.Length > 0))
                {
                    var url = await _imageStorage.SaveHotelImageAsync(img);
                    _context.TblHotelImages.Add(new TblHotelImage
                    {
                        HotelId = entity.HotelId,
                        ImageUrl = url
                    });
                }
                await _context.SaveChangesAsync();
            }

            return (true, "Hotel created successfully.", entity.HotelId);
        }

        public async Task<(bool Success, string Message)> UpdateAsync(UpdateHotelDto dto, IEnumerable<IFormFile>? newImages)
        {
            var entity = await _context.TblHotels.FirstOrDefaultAsync(h => h.HotelId == dto.HotelId);
            if (entity is null)
                return (false, "Hotel not found.");

            var nameExists = await _context.TblHotels
                .AsNoTracking()
                .AnyAsync(h => h.Name == dto.Name && h.HotelId != dto.HotelId);
            if (nameExists)
                return (false, "This hotel name is already in use by another hotel.");

            if (dto.Stars is > 5)
                return (false, "Stars must be between 0 and 5.");

            entity.Name = dto.Name;
            entity.Location = dto.Location;
            entity.Address = dto.Address;
            entity.Stars = dto.Stars;
            entity.Phone = dto.Phone;
            entity.ContactInfo = dto.ContactInfo;
            entity.CheckInTime = dto.CheckInTime;
            entity.CheckOutTime = dto.CheckOutTime;
            entity.CancellationPolicy = dto.CancellationPolicy;
            entity.Fees = dto.Fees;
            entity.Commission = dto.Commission ?? entity.Commission;

            await _context.SaveChangesAsync();

            if (newImages != null)
            {
                foreach (var img in newImages.Where(f => f?.Length > 0))
                {
                    var url = await _imageStorage.SaveHotelImageAsync(img);
                    _context.TblHotelImages.Add(new TblHotelImage
                    {
                        HotelId = entity.HotelId,
                        ImageUrl = url
                    });
                }
                await _context.SaveChangesAsync();
            }

            return (true, "Hotel updated successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int hotelId)
        {
            var entity = await _context.TblHotels
                .Include(h => h.TblHotelImages)
                .FirstOrDefaultAsync(h => h.HotelId == hotelId);

            if (entity is null)
                return (false, "Hotel not found.");

            // Best effort file cleanup
            foreach (var img in entity.TblHotelImages)
            {
                _ = await _imageStorage.DeleteAsync(img.ImageUrl);
            }

            _context.TblHotels.Remove(entity);
            await _context.SaveChangesAsync();

            // Rooms and images are cascade-deleted per FK configuration
            return (true, "Hotel deleted successfully.");
        }

        public async Task<(bool Success, string Message)> RemoveImageAsync(int imageId)
        {
            var entity = await _context.TblHotelImages.FirstOrDefaultAsync(i => i.ImageId == imageId);
            if (entity is null) return (false, "Image not found.");

            _context.TblHotelImages.Remove(entity);
            await _context.SaveChangesAsync();

            _ = await _imageStorage.DeleteAsync(entity.ImageUrl);

            return (true, "Image removed successfully.");
        }

        private static HotelDto MapToDto(TblHotel h) => new HotelDto
        {
            HotelId = h.HotelId,
            Name = h.Name,
            Stars = h.Stars,
            ContactInfo = h.ContactInfo,
            Location = h.Location,
            Address = h.Address,
            Phone = h.Phone,
            CheckInTime = h.CheckInTime,
            CheckOutTime = h.CheckOutTime,
            CancellationPolicy = h.CancellationPolicy,
            Fees = h.Fees,
            Commission = h.Commission,
            CreatedAt = h.CreatedAt,
            Images = h.TblHotelImages?.Select(i => new HotelImageDto
            {
                ImageId = i.ImageId,
                ImageUrl = i.ImageUrl
            }).ToList() ?? new List<HotelImageDto>(),
            Rooms = h.TblRooms?.Select(r => new HotelRoomDto
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                RoomType = r.RoomType,
                Price = r.Price,
                Available = r.Available,
                BedsCount = r.BedsCount,
                MaxGuests = r.MaxGuests
            }).ToList() ?? new List<HotelRoomDto>()
        };
    }
}