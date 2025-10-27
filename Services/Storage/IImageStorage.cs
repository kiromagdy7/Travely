using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Travely.Services.Storage
{
    public interface IImageStorage
    {
        // Saves the image and returns a URL relative to web root (e.g., /uploads/hotels/abc.jpg)
        Task<string> SaveHotelImageAsync(IFormFile file);
        Task<bool> DeleteAsync(string relativeUrl);
    }
}