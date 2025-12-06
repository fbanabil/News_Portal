using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Profile;
using News_Portal.Core.ServiceContracts;
using News_Portal.UI.Filters;

namespace News_Portal.UI.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("[area]/[controller]/[action]")]
    [TypeFilter(typeof(ModelStateValidationFilter))]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IImageService _imageService;
        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ILogger<AccountController> logger, IImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> ProfileDetails()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please login again.";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }
            string DefaultImageUrl = await _imageService.GetDefaultProfileImageUrl();

            ViewBag.DefaultImageUrl = DefaultImageUrl;
            _logger.LogInformation("Default profile image url {0}: " ,DefaultImageUrl);

            return View(user.ToProfileToShowDTO());
        }



        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please login again.";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }

            string DefaultImageUrl = await _imageService.GetDefaultProfileImageUrl();
            ViewBag.DefaultImageUrl = DefaultImageUrl;

            return View(user.ToProfileToUpdateDTO());
        }
    

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileToUpdateDTO profileToUpdate)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please login again.";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }
            user.PersonName = profileToUpdate.PersonName;
            user.PhoneNumber = profileToUpdate.PhoneNumber;

            await _imageService.DeleteFromCloudinary(user.PersonImageUrl);

            user.PersonImageUrl = await _imageService.UploadToCloudinary(profileToUpdate.ProfileImage);

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Message"] = "Profile updated successfully.";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction(nameof(ProfileDetails));
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("EditProfile", profileToUpdate);
            }
        }


        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please login again.";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }
            return View(new PasswordChangeDTO());
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordChangeDTO passwordChangeDTO)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                TempData["Message"] = "User not found. Please login again.";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }

            if(passwordChangeDTO.NewPassword != passwordChangeDTO.ConfirmNewPassword)
            {
                TempData["Message"] = "New password and confirmation do not match.";
                return View(passwordChangeDTO);
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, passwordChangeDTO.CurrentPassword, passwordChangeDTO.NewPassword);
            if (result.Succeeded)
            {
                TempData["Message"] = "Password changed successfully.";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction(nameof(ProfileDetails));
            }
            else
            {
                TempData["Message"] = "Password change failed. Please correct the errors and try again.";
                return View(passwordChangeDTO);
            }
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                TempData["Message"] = "User not found. Please login again.";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }
            
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string resetLink = Url.Action("ResetPassword", "Profile", new { area = "Identity", userId = user.Id, token = token }, Request.Scheme);

            _logger.LogInformation($"Password reset link for user {user.Email}: {resetLink}");
            //TempData["Message"] = "Password reset link has been sent to your email.";
            // Replace this line:
            return Ok(new { message = "Reset link sent to your email" });
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Message"] = "Invalid password reset request.";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }

            var result = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);

            if (!result)
            {
                TempData["Message"] = "Invalid or expired password reset token.";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }

            var model = new ResetPasswordDTO();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmResetPasseord(ResetPasswordDTO resetPasswordDTO)
        {
            if(resetPasswordDTO.Password != resetPasswordDTO.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password and Confirm Password must match");
                return View("ResetPassword", resetPasswordDTO);
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                TempData["Message"] = "Invalid password reset request.";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }

            var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), resetPasswordDTO.Password);

            if (result.Succeeded)
            {
                TempData["Message"] = "Password has been reset successfully.";
                return RedirectToAction("ProfileDetails", "Profile", new { area = "Identity" });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("ResetPassword", resetPasswordDTO);
            }

        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }

            await _imageService.DeleteFromCloudinary(user.PersonImageUrl);
            
            IdentityResult result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Message"] = "Account deleted successfully.";
                await _signInManager.SignOutAsync();
                return Ok(new {message="Succesfully Deleted"});
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction(nameof(ProfileDetails));
            }
        }
    }
}
