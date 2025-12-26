using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.News;
using News_Portal.Core.DTO.Profile;
using News_Portal.Core.Enums;
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

        public AdminActionsController(INewsService newsService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _newsService = newsService;
            _userManager = userManager;
            _signInManager = signInManager;
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
                    TempData["ErrorMessage"] = "User is already an Author.";
                    return RedirectToAction(nameof(Index));
                }
                var result = await _userManager.AddToRoleAsync(user, UserTypes.Author.ToString());
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    TempData["SuccessMessage"] = "Author role added successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add user to Author role.";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["ErrorMessage"] = $"User with email {email} not found.";
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
                    
                    TempData["SuccessMessage"] = "Author role removed successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to remove user from Admin role.";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["ErrorMessage"] = $"User with email {email} not found.";
                return RedirectToAction(nameof(Index));
            }
        }



        [HttpPost]
        public async Task<IActionResult> ChangeNewsStatus(Guid newsId, NewsStatus newStatus)
        {
            bool isUpdated = await _newsService.ChangeNewsStatusAsync(newsId, newStatus);
            if (isUpdated)
            {
                TempData["SuccessMessage"] = "News status updated successfully.";
                return Ok($"News status updated to {newStatus}.");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update news status.";
                return BadRequest("Failed to update news status.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeNewsPriority(Guid newsId, NewsPriority newPriority)
        {
            bool isUpdated = await _newsService.ChangeNewsPriorityAsync(newsId, newPriority);
            if (isUpdated)
            {
                TempData["SuccessMessage"] = "News priority updated successfully.";
                return Ok($"News priority updated to {newPriority}.");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update news priority.";
                return BadRequest("Failed to update news priority.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeNewsType(Guid newsId, NewsType newType)
        {
            bool isUpdated = await _newsService.ChangeNewsTypeAsync(newsId, newType);
            if (isUpdated)
            {
                TempData["SuccessMessage"] = "News type updated successfully.";
                return Ok($"News type updated to {newType}.");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update news type.";
                return BadRequest("Failed to update news type.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNewsPermanently(Guid newsId, string? returnUrl)
        {
            bool isDeleted = await _newsService.DeleteNewsByNewsIdAsync(newsId);
            if (isDeleted)
            {
                TempData["SuccessMessage"] = "News deleted successfully.";
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return Ok("News deleted successfully.");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete news.";
                return BadRequest("Failed to delete news.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> PinNews(Guid newsId)
        {
            bool isPinned = await _newsService.PinNewsAsync(newsId);
            if (isPinned)
            {
                TempData["SuccessMessage"] = "News pinned successfully.";
                return Ok("News pinned successfully.");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to pin news.";
                return BadRequest("Failed to pin news.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UnpinNews(Guid newsId)
        {
            bool isUnpinned = await _newsService.UnpinNewsAsync(newsId);
            if (isUnpinned)
            {
                TempData["SuccessMessage"] = "News unpinned successfully.";
                return Ok("News unpinned successfully.");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to unpin news.";
                return BadRequest("Failed to unpin news.");
            }

        }
    }
}
