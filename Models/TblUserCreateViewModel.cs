using System.ComponentModel.DataAnnotations;

namespace Travely.ViewModels
{
    public class TblUserCreateViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(150)]
        public string Fullname { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;

        [StringLength(50)]
        public string? Phone { get; set; }

        public byte? Age { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [StringLength(50)]
        public string Role { get; set; } = "customer"; // Default value

        public string? Imagepath { get; set; } // هنتعامل مع رفع الصور بعدين لو حابب

        [StringLength(100)]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(30)]
        public string Status { get; set; } = "active"; // Default value
    }
}