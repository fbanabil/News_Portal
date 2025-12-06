using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.ServiceContracts;

namespace News_Portal.UI.ViewComponents
{
    [Authorize]
    public class UserProfileViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageService _imageService;

        public UserProfileViewComponent(UserManager<ApplicationUser> userManager, IImageService imageService)
        {
            _userManager = userManager;
            _imageService = imageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.ProfileImageUrl = await _imageService.GetDefaultProfileImageUrl();

            if (user == null)
            {
                return View("UserProfileImage", user);
            }

            return View("UserProfileImage", user);
        }
    }
}