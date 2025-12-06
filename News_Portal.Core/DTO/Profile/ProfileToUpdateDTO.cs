using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Profile;
using News_Portal.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Profile
{
    public class ProfileToUpdateDTO
    {
        [Required]
        [MaxLength(100)]
        public string? PersonName { get; set; }
        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? ExistingProfileImageUrl { get; set; }

        public IFormFile? ProfileImage { get; set; }

    }
}
public static class ProfileToUpdateExtensions
{
    public static ProfileToUpdateDTO ToProfileToUpdateDTO(this ApplicationUser user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        return new ProfileToUpdateDTO
        {
            PersonName = user.PersonName,
            PhoneNumber = user.PhoneNumber,
            ExistingProfileImageUrl = user.PersonImageUrl
        };
    }
}

