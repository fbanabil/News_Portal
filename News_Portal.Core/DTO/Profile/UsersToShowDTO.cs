using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Account;
using News_Portal.Core.DTO.Profile;
using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Profile
{
    public class UsersToShowDTO
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; } 
        public string? Email { get; set; }
        public List<UserTypes>? UserType { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
public static class UsersToShowDTOExtensions
{
    public static UsersToShowDTO? ToUsersToShowDTO(this ApplicationUser? user)
    {
        if (user == null)
        {
            return null;
        }
        return new UsersToShowDTO
        {
            Id = user.Id,
            Name = user.PersonName,
            Email = user.Email,
            ProfileImageUrl = user.PersonImageUrl
        };
    }

    public static bool IsIncludedInFilter(this ApplicationUser applicationUser, UserFilterParameterDTO? userFilterParameterDTO)
    {
        if (userFilterParameterDTO == null)
        {
            return true;
        }
        bool isEmailMatch = string.IsNullOrEmpty(userFilterParameterDTO.Email) ||
                            (!string.IsNullOrEmpty(applicationUser.Email) &&
                             applicationUser.Email.Contains(userFilterParameterDTO.Email, StringComparison.OrdinalIgnoreCase));
        bool isNameMatch = string.IsNullOrEmpty(userFilterParameterDTO.Name) ||
                           (!string.IsNullOrEmpty(applicationUser.PersonName) &&
                            applicationUser.PersonName.Contains(userFilterParameterDTO.Name, StringComparison.OrdinalIgnoreCase));
        return isEmailMatch && isNameMatch;
    }
}
