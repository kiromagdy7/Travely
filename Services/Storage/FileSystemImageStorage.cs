using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Travely.Services.Storage
{
    public class FileSystemImageStorage : IImageStorage
    {
        private readonly IWebHostEnvironment _env;
        private const string UploadFolder = "uploads/hotels";

        public FileSystemImageStorage(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveHotelImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("Invalid image");

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var relativePath = $"/{UploadFolder}/{fileName}";
            var absoluteFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", UploadFolder);

            Directory.CreateDirectory(absoluteFolder);

            var absolutePath = Path.Combine(absoluteFolder, fileName);
            using (var stream = File.Create(absolutePath))
            {
                await file.CopyToAsync(stream);
            }

            return relativePath.Replace('\\', '/');
        }

        public Task<bool> DeleteAsync(string relativeUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativeUrl)) return Task.FromResult(false);
                var trimmed = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var absolute = Path.Combine(_env.WebRootPath ?? "wwwroot", trimmed);
                if (File.Exists(absolute))
                {
                    File.Delete(absolute);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}