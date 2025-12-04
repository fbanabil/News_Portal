using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Account;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;
using News_Portal.UI.Filters;

namespace News_Portal.UI.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("[controller]/[action]")]
    [TypeFilter(typeof(ModelStateValidationFilter))]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IImageService _imageService;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ILogger<AccountController> logger,IImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _imageService = imageService;
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

            if(user.EmailConfirmed==false)
            {
                ModelState.AddModelError(string.Empty, "Email not confirmed. Please confirm your email before logging in.");
                ViewBag.PresentButton = "Login";
                return View("~/Areas/Identity/Views/Account/AuthLogin.cshtml", loginDTO);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Wrong Password");
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
                PersonImageUrl= "https://res.cloudinary.com/dwkr48bj7/image/upload/User_fiy61j.jpg"
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

            return RedirectToAction(nameof(AuthLogin));
        }



        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData["Error"] = "Invalid email confirmation link.";
                return RedirectToAction(nameof(AuthLogin));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(AuthLogin));
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Message"] = "Email confirmed successfully! You can now log in.";
            }
            else
            {
                TempData["Error"] = "Error confirming email.";
            }

            return RedirectToAction(nameof(AuthLogin));
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        [HttpGet]
        public async Task<IActionResult> IsEmailRegistered(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user?.EmailConfirmed==false)
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


    }
}
