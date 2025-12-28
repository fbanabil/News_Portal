using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Account;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;
using News_Portal.Core.Services;
using News_Portal.UI.Filters;
using System.Security.Claims;

namespace News_Portal.UI.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("[area]/[controller]/[action]")]
    [TypeFilter(typeof(ModelStateValidationFilter))]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IImageService _imageService;
        private readonly IEmailService _emailService;
        private readonly INewsService _newsService;
        private readonly ICommentService _commentService;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ILogger<AccountController> logger, IImageService imageService, IEmailService emailService, INewsService newsService, ICommentService commentService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _imageService = imageService;
            _emailService = emailService;
            _newsService = newsService;
            _commentService = commentService;
        }


        [HttpGet]
        public async Task<IActionResult> AuthLogin()
        {
            ViewBag.PresentButton = "Login";
            LoginDTO loginDTO = new LoginDTO();
            return View("~/Areas/Identity/Views/Account/AuthLogin.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> AuthRegister()
        {
            ViewBag.PresentButton = "Register";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                ViewBag.PresentButton = "Login";
                return View("~/Areas/Identity/Views/Account/AuthLogin.cshtml", loginDTO);
            }

            ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User Not Available");
                ViewBag.PresentButton = "Login";
                return View("~/Areas/Identity/Views/Account/AuthLogin.cshtml", loginDTO);
            }

            if (user.EmailConfirmed == false)
            {
                //ModelState.AddModelError(string.Empty, "Email not confirmed. Please confirm your email before logging in.");
                TempData["Error"] = "Email not confirmed. Please confirm your email before logging in.";
                TempData["ErrorBangla"] = "ইমেইল নিশ্চিত করা হয়নি। লগইন করার আগে অনুগ্রহ করে আপনার ইমেইল নিশ্চিত করুন।";
                ViewBag.PresentButton = "Login";
                return View("~/Areas/Identity/Views/Account/AuthLogin.cshtml", loginDTO);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                //ModelState.AddModelError(string.Empty, "Wrong Password");
                TempData["Error"] = "Wrong Password";
                TempData["ErrorBangla"] = "ভুল পাসওয়ার্ড";
                ViewBag.PresentButton = "Login";
                return View("~/Areas/Identity/Views/Account/AuthLogin.cshtml", loginDTO);
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                ViewBag.PresentButton = "Register";
                return View(registerDTO);
            }
            ApplicationUser newUser = new ApplicationUser()
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PersonName = registerDTO.PersonName,
                PhoneNumber = registerDTO.PhoneNumber,
                EmailConfirmed = false,
                PersonImageUrl = await _imageService.GetDefaultProfileImageUrl()
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, registerDTO.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                ModelState.AddModelError(string.Empty, string.Join(", ", errors));
                ViewBag.PresentButton = "Register";
                return View(registerDTO);
            }
            await _userManager.AddToRoleAsync(newUser, UserTypes.User.ToString());
            // Handling Profile Image Upload
            if (registerDTO.ProfileImage != null && registerDTO.ProfileImage.Length > 0)
            {
                newUser.PersonImageUrl = await _imageService.UploadToCloudinary(registerDTO.ProfileImage);
                await _userManager.UpdateAsync(newUser);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = newUser.Id, token = token }, Request.Scheme);

            _logger.LogInformation("Email confirmation link: {ConfirmationLink}", confirmationLink);

            TempData["Message"] = "Registration successful! Please check your email to confirm your account.";
            TempData["MessageBangla"] = "নিবন্ধন সফল! আপনার অ্যাকাউন্ট নিশ্চিত করতে অনুগ্রহ করে আপনার ইমেইল চেক করুন।";

            return RedirectToAction(nameof(AuthLogin));
        }



        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData["Error"] = "Invalid email confirmation link.";
                TempData["ErrorBangla"] = "অবৈধ ইমেইল নিশ্চিতকরণ লিঙ্ক।";
                return RedirectToAction(nameof(AuthLogin));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                TempData["ErrorBangla"] = "ব্যবহারকারী পাওয়া যায়নি।";
                return RedirectToAction(nameof(AuthLogin));
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Message"] = "Email confirmed successfully! You can now log in.";
                TempData["MessageBangla"] = "ইমেইল সফলভাবে নিশ্চিত হয়েছে! আপনি এখন লগইন করতে পারেন।";
            }
            else
            {
                TempData["Error"] = "Error confirming email.";
                TempData["ErrorBangla"] = "ইমেইল নিশ্চিতকরণে ত্রুটি।";
            }

            return RedirectToAction(nameof(AuthLogin));
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Message"] = "You have been logged out successfully.";
            TempData["MessageBangla"] = "আপনি সফলভাবে লগআউট হয়েছেন।";
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        [HttpGet]
        public async Task<IActionResult> IsEmailRegistered(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user?.EmailConfirmed == false)
            {
                return Json(false);
            }

            if (user != null)
            {
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        public async Task<IActionResult> IsEmailAlreadyRegistered_1(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return Json(false);
            }
            return Json(true);
        }



        [HttpGet]
        public async Task<IActionResult> IsAuthor(string AuthorEmail)
        {
            var user = await _userManager.FindByEmailAsync(AuthorEmail);
            if (user != null)
            {
                var isAuthor = await _userManager.IsInRoleAsync(user, UserTypes.Author.ToString());
                var isAdmin = await _userManager.IsInRoleAsync(user, UserTypes.Admin.ToString());
                if (isAuthor || isAdmin)
                {
                    return Json(true);
                }
            }
            return Json(false);
        }




        [HttpGet]
        public async Task<IActionResult> IsAnAuthor([FromQuery(Name = "authorAddRemoveDTO.RemoveEmail")]string RemoveEmail)
        {
            var user = await _userManager.FindByEmailAsync(RemoveEmail);
            if (user != null)
            {
                var isAuthor = await _userManager.IsInRoleAsync(user, UserTypes.Author.ToString());
                if (isAuthor)
                {
                    return Json(true);
                }
            }
            return Json(false);
        }


        [HttpGet]
        public async Task<IActionResult> ValidAuthorEmailToAdd([FromQuery(Name = "authorAddRemoveDTO.AddEmail")]string AddEmail)
        {
            var user = await _userManager.FindByEmailAsync(AddEmail);
            if(user == null)
            {
                return Json(false);
            }
            var isAuthor = await _userManager.IsInRoleAsync(user, UserTypes.Author.ToString());
            if (isAuthor)
            {
                return Json(false);
            }
            return Json(true);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null)
        {
            string redirectUrl = Request.Scheme.ToString() + "://" + Request.Host.ToString() + "/Identity/Account/ExternalLoginCallback";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }




        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                ViewBag.PresentButton = "Login";
                return View("~/Areas/Identity/Views/Account/AuthLogin.cshtml", new LoginDTO());
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                ViewBag.PresentButton = "Login";
                return View("~/Areas/Identity/Views/Account/AuthLogin.cshtml", new LoginDTO());
            }
            var email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);


            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                TempData["Message"] = "Logged in successfully!";
                TempData["MessageBangla"] = "সফলভাবে লগইন হয়েছে!";
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            

            if (email == null)
            {
                ModelState.AddModelError(string.Empty, "Email claim not received from external provider.");
                ViewBag.PresentButton = "Login";
                return View("~/Areas/Identity/Views/Account/AuthLogin.cshtml", new LoginDTO());
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    PersonName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email,
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    PersonImageUrl = await _imageService.GetDefaultProfileImageUrl()
                };
                string randomPassword = new string(Enumerable.Range(0, 10)
                    .Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*"[Random.Shared.Next(68)])
                    .ToArray());
                _logger.LogInformation(randomPassword);
                IdentityResult res = await _userManager.CreateAsync(user, randomPassword);

                if (!res.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Something Went Wrong.");
                    ViewBag.PresentButton = "Login";
                    return View("~/Areas/Identity/Views/Account/AuthLogin.cshtml", new LoginDTO());
                }

                string emailToSend = $"Your account has been created. Your temporary password is {randomPassword}. Please reset your password after logging in.";
                await _emailService.SendEmailAsync(email, "Account Created", emailToSend);

                TempData["Message"] = "Account created successfully . Password sent you email";
                TempData["MessageBangla"] = "অ্যাকাউন্ট সফলভাবে তৈরি হয়েছে। পাসওয়ার্ড আপনার ইমেইলে পাঠানো হয়েছে।";
                await _userManager.AddToRoleAsync(user, UserTypes.User.ToString());
            }
            if (user.EmailConfirmed == false)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home", new { area = "" });

        }


        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View(new ForgotPasswordDTO());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPasswordReset(ForgotPasswordDTO forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                TempData["Error"] = "Invalid Request";
                TempData["ErrorBangla"] = "অবৈধ অনুরোধ";
                return RedirectToAction(nameof(ForgotPassword));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { token = token, email = forgotPasswordDto.Email }, Request.Scheme);
            _logger.LogInformation("Password reset link: {ResetLink}", resetLink);

            await _emailService.SendEmailAsync(forgotPasswordDto.Email, "Password Reset", $"Please reset your password by <a href='{System.Net.WebUtility.HtmlEncode(resetLink)}'>clicking here</a>.");

            TempData["Message"] = "Password reset link has been sent to your email.";
            TempData["MessageBangla"] = "পাসওয়ার্ড রিসেট লিঙ্ক আপনার ইমেইলে পাঠানো হয়েছে।";
            return RedirectToAction(nameof(ForgotPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            ForgotPasswordResetDTO forgotPasswordResetDTO = new ForgotPasswordResetDTO()
            {
                Email = email,
                Token = token
            };
            return View(forgotPasswordResetDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmResetPassword(ForgotPasswordResetDTO forgotPasswordResetDTO)
        {
            if (string.IsNullOrEmpty(forgotPasswordResetDTO.Email) || string.IsNullOrEmpty(forgotPasswordResetDTO.Token))
            {
                TempData["Error"] = "Invalid password reset request.";
                TempData["ErrorBangla"] = "অবৈধ পাসওয়ার্ড রিসেট অনুরোধ।";
                return RedirectToAction(nameof(ForgotPassword));
            }

            if (forgotPasswordResetDTO.NewPassword != forgotPasswordResetDTO.ConfirmNewPassword)
            {
                TempData["Error"] = "New password and confirmation do not match.";
                TempData["ErrorBangla"] = "নতুন পাসওয়ার্ড এবং নিশ্চিতকরণ মেলে না।";
                return View("ResetPassword", forgotPasswordResetDTO);
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordResetDTO.Email);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                TempData["ErrorBangla"] = "ব্যবহারকারী পাওয়া যায়নি।";
                return RedirectToAction(nameof(ForgotPassword));
            }

            var result = await _userManager.ResetPasswordAsync(user, forgotPasswordResetDTO.Token, forgotPasswordResetDTO.NewPassword);

            if (result.Succeeded)
            {
                TempData["Message"] = "Your password has been reset successfully. You can now log in with your new password.";
                TempData["MessageBangla"] = "আপনার পাসওয়ার্ড সফলভাবে রিসেট হয়েছে। আপনি এখন আপনার নতুন পাসওয়ার্ড দিয়ে লগইন করতে পারেন।";
                return RedirectToAction(nameof(AuthLogin));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("ResetPassword", forgotPasswordResetDTO);
            }
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            ViewData["Message"] = "You do not have permission to access this resource.";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            await _imageService.DeleteFromCloudinary(user.PersonImageUrl);

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
