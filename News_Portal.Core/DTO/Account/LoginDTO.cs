using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Account
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required | ইমেইল প্রয়োজন")]
        [EmailAddress(ErrorMessage = "Invalid email address | অবৈধ ইমেইল ঠিকানা")]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "IsEmailRegistered", controller: "Account", areaName: "Identity", ErrorMessage = "Not registered or verified | নিবন্ধিত বা যাচাইকৃত নয়")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required | পাসওয়ার্ড প্রয়োজন")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
