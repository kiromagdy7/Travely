using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // <-- Required for IFormFile

namespace Travely.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(150)]
        public string Fullname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = string.Empty; // Assuming TblUser.Phone is string

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a role")]
        [Display(Name = "Register As")]
        public string Role { get; set; } = "customer"; // Default value

        // --- Profile Picture Property ---
        [Display(Name = "Profile Picture")]
        // Add validation attributes if needed (e.g., file size, type)
        public IFormFile? ProfileImage { get; set; } 
    }
}