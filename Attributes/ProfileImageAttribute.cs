using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Travely.Attributes
{
    public class ProfileImageAttribute : ValidationAttribute
    {
        public int MaxSizeInMb { get; set; } = 4;
        public string[] AllowedExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // File is optional
            }

            var file = value as IFormFile;
            if (file == null)
            {
                return new ValidationResult("Invalid file upload");
            }

            // Check file size
            if (file.Length > MaxSizeInMb * 1024 * 1024)
            {
                return new ValidationResult($"File size must not exceed {MaxSizeInMb}MB");
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return new ValidationResult($"Only {string.Join(", ", AllowedExtensions)} files are allowed");
            }

            return ValidationResult.Success;
        }
    }
}
