using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Profile
{
    public class AuthorsToShowDTO
    {
        public string? AuthorName { get; set; }
        public string? AuthorEmail { get; set; }
    }
}
public static class AuthorsToShowDTOExtensions
{
    public static AuthorsToShowDTO ToAuthorsToShowDTO(this ApplicationUser applicationUser)
    {
        return new AuthorsToShowDTO
        {
            AuthorName = applicationUser.UserName,
            AuthorEmail = applicationUser.Email
        };
    }
}
