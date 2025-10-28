using System.Collections.Generic;
using Travely.Models; // <-- ضيف ده عشان TblWishList

namespace Travely.ViewModels
{
    public class WishlistPageViewModel
    {
        // --- 1. بيانات اليوزر (عشان الـ Sidebar) ---
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public int? Age { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string Imagepath { get; set; }

        // --- 2. بيانات الـ Wishlist (عشان المحتوى الرئيسي) ---
        public IEnumerable<TblWishList> WishlistItems { get; set; }

        public WishlistPageViewModel()
        {
            WishlistItems = new List<TblWishList>();
        }
    }
}