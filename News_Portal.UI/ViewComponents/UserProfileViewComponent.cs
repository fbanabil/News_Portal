using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Domain.IdentityEntities;

namespace News_Portal.UI.ViewComponents
{
    [Authorize]
    public class UserProfileViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.ProfileImageUrl = "https://res.cloudinary.com/dwkr48bj7/image/upload/User_fiy61j.jpg";

            if (user == null)
            {
                return View("UserProfileImage", user);
            }

            return View("UserProfileImage", user);
        }
    }
}