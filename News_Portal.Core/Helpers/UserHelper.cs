using Microsoft.AspNetCore.Identity;
using News_Portal.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Helpers
{
    public class UserHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserHelper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public bool IsUserInRoleAsync(ApplicationUser user, string role)
        {
            if(role == null) 
            {
                return true;   
            }

            return _userManager.IsInRoleAsync(user, role).Result;
        }
    }
}
