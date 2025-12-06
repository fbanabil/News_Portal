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
        [Required]
        [Remote(action: "IsEmailRegistered",controller:"Account",areaName:"Identity", ErrorMessage = "Email Not Registered")]
        public string? Email { get; set; }

    }
}
