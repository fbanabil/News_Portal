using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Account;
using News_Portal.Core.DTO.News;
using News_Portal.Core.DTO.Profile;
using News_Portal.Core.Enums;
using News_Portal.Core.Helpers;
using News_Portal.Core.ServiceContracts;
using News_Portal.UI.Filters;

namespace News_Portal.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    [TypeFilter(typeof(ModelStateValidationFilter))]
    [Authorize(Roles = "Admin")]
    public class AdminActionsController : Controller
    {

        private readonly INewsService _newsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserHelper _userHelper;

        public AdminActionsController(INewsService newsService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _newsService = newsService;
            _userManager = userManager;
            _signInManager = signInManager;
            _userHelper = new UserHelper(_userManager);
        }





        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] NewsFilterParametersDTO? parametersDTO, [FromQuery] string? AuthorEmail, [FromQuery] string? sortBy = "PublishedDate", [FromQuery] SortTypes sortType = SortTypes.Default, [FromQuery] int pageNo = 1, [FromQuery] int pageSize = 10)
        {
            AdminNewsAllFiltersDTO adminNewsAllFiltersDTO = new AdminNewsAllFiltersDTO();
            adminNewsAllFiltersDTO.parametersDTO = parametersDTO ?? new NewsFilterParametersDTO();
            adminNewsAllFiltersDTO.sortBy = sortBy ?? "PublishedDate";
            adminNewsAllFiltersDTO.sortType = sortType;
            adminNewsAllFiltersDTO.pageNo = pageNo;
            adminNewsAllFiltersDTO.pageSize = pageSize;
            adminNewsAllFiltersDTO.AuthorEmail = AuthorEmail;


            int totalNewsCount = 0;

            if (AuthorEmail != null)
            {
                ApplicationUser? author = await _userManager.FindByEmailAsync(AuthorEmail);
                if (author != null) totalNewsCount = await _newsService.GetAdminPageNewsCountAsync(author.Id, parametersDTO);
            }
            else
            {
                totalNewsCount = await _newsService.GetAdminPageNewsCountAsync(null, parametersDTO);
            }
            ViewBag.PageCount = (int)Math.Ceiling((double)totalNewsCount / pageSize);


            IList<ApplicationUser>? authors = await _userManager.GetUsersInRoleAsync(UserTypes.Author.ToString());
            List<AuthorsToShowDTO> allAuthors = authors.Select(u => u.ToAuthorsToShowDTO()).OrderBy(x => x.AuthorName).ToList();
            ViewBag.AuthorsList = new SelectList(allAuthors, "AuthorEmail", "AuthorName", AuthorEmail);

            return View(adminNewsAllFiltersDTO);
        }




        [HttpGet]
        public IActionResult AdminNewsList(NewsFilterParametersDTO NewsFilterParametersDTO, string sortBy = "PublishedDate", SortTypes sortOptions = SortTypes.Default, int pageNo = 1, int pageSize = 10)
        {
            return ViewComponent("AdminNews", new
            {
                NewsFilterParametersDTO,
                sortBy,
                sortOptions,
                pageNo,
                pageSize
            });
        }






        [HttpGet]
        public async Task<IActionResult> DetailedNews(Guid newsId)
        {
            AdminsNewsDetailesToShowDTO? adminsNewsDetailedDTO = await _newsService.GetAdminsNewsDetailsAsync(newsId);
            if (adminsNewsDetailedDTO == null)
            {
                return null;
            }
            string? videoLink = adminsNewsDetailedDTO.VideoUrl ?? "";
            string youtubeEmbedUrl = "";
            if (videoLink.Contains("youtube.com/watch?v="))
            {

                var parts = videoLink.Split("watch?v=", StringSplitOptions.RemoveEmptyEntries);
                var idPart = parts.Last().Split('&')[0];
                youtubeEmbedUrl = $"https://www.youtube.com/embed/{idPart}";
            }
            else if (videoLink.Contains("youtu.be/"))
            {
                var parts = videoLink.Split("youtu.be/", StringSplitOptions.RemoveEmptyEntries);
                var idPart = parts.Last().Split('?')[0];
                youtubeEmbedUrl = $"https://www.youtube.com/embed/{idPart}";
            }
            ViewBag.VideoLink = youtubeEmbedUrl;
            adminsNewsDetailedDTO.VideoUrl = youtubeEmbedUrl;
            ViewBag.adminsNewsDetailedDTO = adminsNewsDetailedDTO;

            bool isPined = await _newsService.IsNewsPinnedAsync(newsId);
            ViewBag.IsPinned = isPined;

            return View(adminsNewsDetailedDTO);
        }





        [HttpPost]
        public async Task<IActionResult> AddAuthor([FromForm(Name = "authorAddRemoveDTO.AddEmail")]string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {  
                if (await _userManager.IsInRoleAsync(user, UserTypes.Author.ToString()))
                {
                    TempData["Error"] = "User is already an Author.";
                    TempData["ErrorBangla"] = "ব্যবহারকারী ইতিমধ্যেই একজন লেখক।";

                    return RedirectToAction(nameof(Index));
                }
                var result = await _userManager.AddToRoleAsync(user, UserTypes.Author.ToString());
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    TempData["Message"] = "Author role added successfully.";
                    TempData["MessageBangla"] = "লেখক ভূমিকা সফলভাবে যোগ করা হয়েছে।";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Failed to add user to Author role.";
                    TempData["ErrorBangla"] = "ব্যবহারকারীকে লেখক ভূমিকা যোগ করতে ব্যর্থ হয়েছে।";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["Error"] = $"User with email {email} not found.";
                TempData["ErrorBangla"] = $"{email} ইমেইল সহ ব্যবহারকারী পাওয়া যায়নি।";
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        public async Task<IActionResult> RemoveAuthor([FromForm(Name = "authorAddRemoveDTO.RemoveEmail")] string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, UserTypes.Author.ToString()))
                {
                    return BadRequest("User is not an Author.");
                }
                var result = await _userManager.RemoveFromRoleAsync(user, UserTypes.Author.ToString());
                if (result.Succeeded)
                {
                    //if author removed he must autometically logget out is logged in

                    await _userManager.UpdateSecurityStampAsync(user);
                    
                    TempData["Message"] = "Author role removed successfully.";
                    TempData["MessageBangla"] = "লেখক ভূমিকা সফলভাবে সরানো হয়েছে।";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Failed to remove user from Admin role.";
                    TempData["ErrorBangla"] = "ব্যবহারকারীকে প্রশাসক ভূমিকা থেকে সরাতে ব্যর্থ হয়েছে।";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["Error"] = $"User with email {email} not found.";
                TempData["ErrorBangla"] = $"{email} ইমেইল সহ ব্যবহারকারী পাওয়া যায়নি।";
                return RedirectToAction(nameof(Index));
            }
        }



        [HttpPost]
        public async Task<IActionResult> ChangeNewsStatus(Guid newsId, NewsStatus newStatus)
        {
            bool isUpdated = await _newsService.ChangeNewsStatusAsync(newsId, newStatus);
            if (isUpdated)
            {
                TempData["Message"] = "News status updated successfully.";
                TempData["MessageBangla"] = "সংবাদের অবস্থা সফলভাবে আপডেট হয়েছে।";
                return Ok($"News status updated to {newStatus}.");
            }
            else
            {
                TempData["Error"] = "Failed to update news status.";
                TempData["ErrorBangla"] = "সংবাদের অবস্থা আপডেট করতে ব্যর্থ হয়েছে।";
                return BadRequest("Failed to update news status.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeNewsPriority(Guid newsId, NewsPriority newPriority)
        {
            bool isUpdated = await _newsService.ChangeNewsPriorityAsync(newsId, newPriority);
            if (isUpdated)
            {
                TempData["Success"] = "News priority updated successfully.";
                TempData["SuccessBangla"] = "সংবাদের অগ্রাধিকার সফলভাবে আপডেট হয়েছে।";
                return Ok($"News priority updated to {newPriority}.");
            }
            else
            {
                TempData["Error"] = "Failed to update news priority.";
                TempData["ErrorBangla"] = "সংবাদের অগ্রাধিকার আপডেট করতে ব্যর্থ হয়েছে।";
                return BadRequest("Failed to update news priority.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeNewsType(Guid newsId, NewsType newType)
        {
            bool isUpdated = await _newsService.ChangeNewsTypeAsync(newsId, newType);
            if (isUpdated)
            {
                TempData["Success"] = "News type updated successfully.";
                TempData["SuccessBangla"] = "সংবাদের ধরন সফলভাবে আপডেট হয়েছে।";
                return Ok($"News type updated to {newType}.");
            }
            else
            {
                TempData["Error"] = "Failed to update news type.";
                TempData["ErrorBangla"] = "সংবাদের ধরন আপডেট করতে ব্যর্থ হয়েছে।";
                return BadRequest("Failed to update news type.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNewsPermanently(Guid newsId, string? returnUrl)
        {
            bool isDeleted = await _newsService.DeleteNewsByNewsIdAsync(newsId);
            if (isDeleted)
            {
                TempData["Success"] = "News deleted successfully.";
                TempData["SuccessBangla"] = "সংবাদ সফলভাবে মুছে ফেলা হয়েছে।";
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return Ok("News deleted successfully.");
            }
            else
            {
                TempData["Error"] = "Failed to delete news.";
                TempData["ErrorBangla"] = "সংবাদ মুছে ফেলতে ব্যর্থ হয়েছে।";
                return BadRequest("Failed to delete news.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> PinNews(Guid newsId)
        {
            bool isPinned = await _newsService.PinNewsAsync(newsId);
            if (isPinned)
            {
                TempData["Success"] = "News pinned successfully.";
                TempData["SuccessBangla"] = "সংবাদ সফলভাবে পিন করা হয়েছে।";
                return Ok("News pinned successfully.");
            }
            else
            {
                TempData["Error"] = "Failed to pin news.";
                TempData["ErrorBangla"] = "সংবাদ পিন করতে ব্যর্থ হয়েছে।";
                return BadRequest("Failed to pin news.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UnpinNews(Guid newsId)
        {
            bool isUnpinned = await _newsService.UnpinNewsAsync(newsId);
            if (isUnpinned)
            {
                TempData["Success"] = "News unpinned successfully.";
                TempData["SuccessBangla"] = "সংবাদ সফলভাবে আনপিন করা হয়েছে।";
                return Ok("News unpinned successfully.");
            }
            else
            {
                TempData["Error"] = "Failed to unpin news.";
                TempData["ErrorBangla"] = "সংবাদ আনপিন করতে ব্যর্থ হয়েছে।";
                return BadRequest("Failed to unpin news.");
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetUsers(UserFilterParameterDTO? userFilterParameterDTO, int pageNo = 1, int pageSize = 5)
        {
            List<ApplicationUser> users = await _userManager.Users.AsNoTracking().ToListAsync();

            List<ApplicationUser> filteredUsers = users.Where(u => u.IsIncludedInFilter(userFilterParameterDTO)).ToList();

            if (userFilterParameterDTO?.UserType != null)
            {
                filteredUsers = filteredUsers.Where(u => _userHelper.IsUserInRoleAsync(u, userFilterParameterDTO.UserType.ToString())).ToList();
            }
            int totalUsersCount = filteredUsers.Count;


            ViewBag.PageCount = (int)Math.Ceiling((double)totalUsersCount / pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = 1;

            IList<ApplicationUser>? authors = await _userManager.GetUsersInRoleAsync(UserTypes.Author.ToString());
            List<AuthorsToShowDTO> allAuthors = authors.Select(u => u.ToAuthorsToShowDTO()).OrderBy(x => x.AuthorName).ToList();
            ViewBag.AuthorsList = new SelectList(allAuthors, "AuthorEmail", "AuthorName");

            return View(userFilterParameterDTO);
        }

        [HttpGet]
        public async Task<IActionResult> UsersList(UserFilterParameterDTO? userFilterParameterDTO, int pageNo = 1, int pageSize = 5)
        {
            return ViewComponent("Users", new
            {
                userFilterParameterDTO,
                pageNo,
                pageSize
            });
        }


    }
}
