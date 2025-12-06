using System.ComponentModel.DataAnnotations;

namespace News_Portal.Core.DTO.Profile
{
    public class PasswordChangeDTO
    {
        [Required(ErrorMessage = "Current Password must be given")]
        public string? CurrentPassword { get; set; }
        
        [Required(ErrorMessage = "New Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string? NewPassword { get; set; }
        
        [Required(ErrorMessage = "Please confirm your new password")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm New Password must match")]
        public string? ConfirmNewPassword { get; set; }
    }
}
