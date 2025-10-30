﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Travely.Attributes;

namespace Travely.ViewModels
{
    public class ProfileEditViewModel
    {
        // دي البيانات اللي هنعرضها ونعدلها
        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 150 characters")]
        [RegularExpression(@"^[a-zA-Z\s-']+$", ErrorMessage = "Full name can only contain letters, spaces, hyphens and apostrophes")]
        public string Fullname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters")]
        public string? Country { get; set; }

        [Display(Name = "Age")]
        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
        public int? Age { get; set; }

        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Please enter a valid phone number (10-15 digits, may start with +)")]
        public string Phone { get; set; } = string.Empty;

        public string? CurrentImagePath { get; set; }

        [Display(Name = "Profile Picture")]
        // File size validation will be handled in custom attribute
        [ProfileImage(MaxSizeInMb = 4, AllowedExtensions = new[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile? NewImage { get; set; }
    }
}