using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Account
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Email is required | ইমেইল প্রয়োজন")]
        [Remote(action: "IsEmailRegistered", controller: "Account", areaName: "Identity", ErrorMessage = "Email not registered | ইমেইল নিবন্ধিত নয়")]
        public string? Email { get; set; }

    }
}
