using Microsoft.AspNetCore.Http; // ضروري علشان IFormFile
using System.ComponentModel.DataAnnotations;

namespace Travely.ViewModels
{
    public class ProfileEditViewModel
    {
        // دي البيانات اللي هنعرضها ونعدلها
        [Required]
        [Display(Name = "Full Name")]
        public string Fullname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Country { get; set; }

        public int? Age { get; set; } // خليته nullable احتياطي

        public string Phone { get; set; }

        // --- قسم الصورة ---

        // ده هنخزن فيه المسار الحالي للصورة عشان نعرضها
        public string? CurrentImagePath { get; set; }

        // ده هيستقبل الصورة الجديدة اللي اليوزر هيرفعها
        [Display(Name = "Profile Picture")]
        public IFormFile? NewImage { get; set; }
    }
}