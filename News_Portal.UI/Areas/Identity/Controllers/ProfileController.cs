using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Profile;
using News_Portal.Core.Enums;
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
        private readonly INewsService _newsService;
        private readonly ICommentService _commentService;
        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ILogger<AccountController> logger, IImageService imageService, INewsService newsService, ICommentService commentService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _imageService = imageService;
            _newsService = newsService;
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> ProfileDetails()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please login again.";
                TempData["ErrorBangla"] = "ব্যবহারকারী পাওয়া যায়নি। দয়া করে আবার লগইন করুন।";
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
                TempData["ErrorBangla"] = "ব্যবহারকারী পাওয়া যায়নি। দয়া করে আবার লগইন করুন।";
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
                TempData["ErrorBangla"] = "ব্যবহারকারী পাওয়া যায়নি। দয়া করে আবার লগইন করুন।";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }
            user.PersonName = profileToUpdate.PersonName;
            user.PhoneNumber = profileToUpdate.PhoneNumber;

            if(user.PersonImageUrl!=null) await _imageService.RemoveImage(user.PersonImageUrl);

            if(profileToUpdate.ProfileImage!=null) user.PersonImageUrl = await _imageService.SaveProfileImage(profileToUpdate.ProfileImage);

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Message"] = "Profile updated successfully.";
                TempData["MessageBangla"] = "প্রোফাইল সফলভাবে আপডেট হয়েছে।";
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
                TempData["ErrorBangla"] = "ব্যবহারকারী পাওয়া যায়নি। দয়া করে আবার লগইন করুন।";
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
                TempData["MessageBangla"] = "ব্যবহারকারী পাওয়া যায়নি। দয়া করে আবার লগইন করুন।";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }

            if(passwordChangeDTO.NewPassword != passwordChangeDTO.ConfirmNewPassword)
            {
                TempData["Message"] = "New password and confirmation do not match.";
                TempData["MessageBangla"] = "নতুন পাসওয়ার্ড এবং নিশ্চিতকরণ মেলে না।";
                return View(passwordChangeDTO);
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, passwordChangeDTO.CurrentPassword, passwordChangeDTO.NewPassword);
            if (result.Succeeded)
            {
                TempData["Message"] = "Password changed successfully.";
                TempData["MessageBangla"] = "পাসওয়ার্ড সফলভাবে পরিবর্তন হয়েছে।";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction(nameof(ProfileDetails));
            }
            else
            {
                TempData["Message"] = "Password change failed. Please correct the errors and try again.";
                TempData["MessageBangla"] = "পাসওয়ার্ড পরিবর্তন ব্যর্থ হয়েছে। দয়া করে ত্রুটিগুলি সংশোধন করুন এবং আবার চেষ্টা করুন।";
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
                TempData["MessageBangla"] = "ব্যবহারকারী পাওয়া যায়নি। দয়া করে আবার লগইন করুন।";
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
                TempData["MessageBangla"] = "অবৈধ পাসওয়ার্ড রিসেট অনুরোধ।";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }

            var result = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);

            if (!result)
            {
                TempData["Message"] = "Invalid or expired password reset token.";
                TempData["MessageBangla"] = "অবৈধ বা মেয়াদোত্তীর্ণ পাসওয়ার্ড রিসেট টোকেন।";
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
                TempData["MessageBangla"] = "অবৈধ পাসওয়ার্ড রিসেট অনুরোধ।";
                return RedirectToAction("AuthLogin", "Account", new { area = "Identity" });
            }

            var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), resetPasswordDTO.Password);

            if (result.Succeeded)
            {
                TempData["Message"] = "Password has been reset successfully.";
                TempData["MessageBangla"] = "পাসওয়ার্ড সফলভাবে রিসেট হয়েছে।";
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest(new { success = false, message = "User not authenticated.", redirect = Url.Action("AuthLogin", "Account", new { area = "Identity" }) });
            }

            await _imageService.RemoveImage(user.PersonImageUrl);

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(UserTypes.Admin.ToString()))
            {
                return BadRequest(new { success = false, message = "Cannot delete the only admin account.", messageBangla = "একক অ্যাডমিন অ্যাকাউন্ট মুছে ফেলা যাবে না।" });
            }

            await _newsService.DeleteNewsByAuthorId(user.Id);         
            await _commentService.DeleteCommentsByUserId(user.Id);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                TempData["Message"] = "Account deleted successfully.";
                TempData["MessageBangla"] = "অ্যাকাউন্ট সফলভাবে মুছে ফেলা হয়েছে।";
                return Ok(new { success = true, message = "Account deleted successfully." });
            }
            else
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                TempData["Error"] = "Account deletion failed.";
                TempData["ErrorBangla"] = "অ্যাকাউন্ট মুছে ফেলা ব্যর্থ হয়েছে।";
                return BadRequest(new { success = false, message = "Account deletion failed. " + errors });
            }
        }
    }
}
