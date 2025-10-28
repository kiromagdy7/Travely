using System;
using System.Collections.Generic;

namespace Travely.ViewModels
{
    // ده الـ Model الرئيسي للصفحة
    public class ProfileViewModel
    {
        // بيانات اليوزر اللي جاية من View
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public int? Age { get; set; } // خليته nullable احتياطي
        public string Role { get; set; }
        public string Phone { get; set; }
        public string Imagepath { get; set; }

        // قائمة الحجوزات
        public List<BookingInfoViewModel> PastBookings { get; set; }

        public ProfileViewModel()
        {
            PastBookings = new List<BookingInfoViewModel>();
        }
    }
}