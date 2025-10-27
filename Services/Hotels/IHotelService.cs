using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Travely.Dtos.Hotels;

namespace Travely.Services.Hotels
{
    public interface IHotelService
    {
        Task<List<HotelDto>> GetAllAsync(bool includeRooms = false);
        Task<HotelDto?> GetByIdAsync(int hotelId, bool includeRooms = true);
        Task<(bool Success, string Message, int? HotelId)> CreateAsync(CreateHotelDto dto, IEnumerable<IFormFile>? images);
        Task<(bool Success, string Message)> UpdateAsync(UpdateHotelDto dto, IEnumerable<IFormFile>? newImages);
        Task<(bool Success, string Message)> DeleteAsync(int hotelId);
        Task<(bool Success, string Message)> RemoveImageAsync(int imageId);
    }
}