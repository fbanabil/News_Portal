using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Profile
{
    public class ProfileToShowDTO
    {
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }

    }
}

public static class ProfileExtensions
{
    public static ProfileToShowDTO ToProfileToShowDTO(this ApplicationUser user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        return new ProfileToShowDTO
        {
            PersonName = user.PersonName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfileImageUrl = user.PersonImageUrl
        };
    }
}