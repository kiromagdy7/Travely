using System.ComponentModel.DataAnnotations;

namespace Travely.ViewModels
{
    public class RegisterViewModel
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
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        // === START: الإضافة الجديدة ===
        [Required(ErrorMessage = "Please select a role")]
        [Display(Name = "Register As")]
        public string Role { get; set; } = "customer"; // القيمة الافتراضية
        // === END: الإضافة الجديدة ===
    }
}