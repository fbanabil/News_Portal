using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Account
{
    public class ForgotPasswordResetDTO
    {
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}
