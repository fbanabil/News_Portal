using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Account
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email can't be empty | ইমেইল খালি রাখা যাবে না")]
        [EmailAddress(ErrorMessage = "Invalid email address | অবৈধ ইমেইল ঠিকানা")]
        [Remote(action: "IsEmailAlreadyRegistered_1", controller: "Account", areaName: "Identity", ErrorMessage = "Email already taken | ইমেইল ইতিমধ্যে ব্যবহৃত হয়েছে")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Person name can't be empty | নাম খালি রাখা যাবে না")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Phone number can't be empty | ফোন নম্বর খালি রাখা যাবেনা")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain 0-9 | ফোন নম্বর ০-৯ সংখ্যা থাকতে হবে")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password can't be empty | পাসওয়ার্ড খালি রাখা যাবে না")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm password can't be empty | নিশ্চিত পাসওয়ার্ড খালি রাখা যাবে না")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password must be same | পাসওয়ার্ড এবং নিশ্চিত পাসওয়ার্ড একই হতে হবে")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Profile image is required | প্রোফাইল ছবি প্রয়োজন")]
        [DataType(DataType.Upload)]
        public IFormFile? ProfileImage { get; set; }
    }
}
