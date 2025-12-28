using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Account;
using News_Portal.Core.DTO.Profile;
using News_Portal.Core.Enums;
using News_Portal.Core.Helpers;
using System.Linq;

namespace News_Portal.UI.Areas.Admin.ViewComponents
{
    public class UsersViewComponent :ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserHelper _userHelper;

        public UsersViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _userHelper = new UserHelper(_userManager);
        }

        public async Task<IViewComponentResult> InvokeAsync(UserFilterParameterDTO? userFilterParameterDTO, int pageNo = 1, int pageSize = 5)
        {
            List<ApplicationUser> users = await _userManager.Users.AsNoTracking().ToListAsync();

            List<ApplicationUser> filteredUsers = users.OrderBy(o => o.PersonName).Where(u => u.IsIncludedInFilter(userFilterParameterDTO)).ToList();

            if (userFilterParameterDTO?.UserType != null)
            {
                filteredUsers = filteredUsers.Where(u => _userHelper.IsUserInRoleAsync(u, userFilterParameterDTO.UserType.ToString())).ToList();
            }
            filteredUsers = filteredUsers.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            List<UsersToShowDTO> usersToShowDTOs = filteredUsers.Select(u => u!.ToUsersToShowDTO()!).ToList();

            foreach (var userDTO in usersToShowDTOs)
            {
                var user = await _userManager.FindByIdAsync(userDTO.Id.ToString());
                if (user != null)
                {
                    userDTO.UserType = new List<UserTypes>();
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains(UserTypes.Admin.ToString()))
                    {
                        userDTO?.UserType?.Add(UserTypes.Admin);
                    }
                    if (roles.Contains(UserTypes.User.ToString()))
                    {
                        userDTO?.UserType?.Add(UserTypes.User);
                    }
                    
                    if(roles.Contains(UserTypes.Author.ToString()))
                    {
                        userDTO?.UserType?.Add(UserTypes.Author);
                    }
                }
            }
            ViewBag.CurrentPage = pageNo;
            return View("UsersViewComponent",usersToShowDTOs);
        }
    }
}
