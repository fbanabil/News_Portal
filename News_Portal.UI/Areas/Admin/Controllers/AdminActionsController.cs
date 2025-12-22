using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.News;
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

        public AdminActionsController(INewsService newsService, UserManager<ApplicationUser> userManager)
        {
            _newsService = newsService;
            _userManager = userManager;
        }





        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] NewsFilterParametersDTO? parametersDTO, [FromQuery] string? sortBy = "PublishedDate", [FromQuery] SortTypes sortType = SortTypes.Default, [FromQuery] int pageNo = 1, [FromQuery] int pageSize = 5)
        {
            AuthorNewsAllFiltersDTO authorNewsAllFiltersDTO = new AuthorNewsAllFiltersDTO();
            authorNewsAllFiltersDTO.parametersDTO = parametersDTO ?? new NewsFilterParametersDTO();
            authorNewsAllFiltersDTO.sortBy = sortBy ?? "PublishedDate";
            authorNewsAllFiltersDTO.sortType = sortType;
            authorNewsAllFiltersDTO.pageNo = 1;
            authorNewsAllFiltersDTO.pageSize = pageSize;

            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);

            List<AuthorsNewsToShowDTO> authorsNewsToShowDTOs = await _newsService.GetAllAuthorsNewsAsync(user.Id, parametersDTO, sortBy, sortType, pageNo, pageSize);

            int totalNewsCount = await _newsService.GetAuthorsNewsCountAsync(user.Id, parametersDTO);
            ViewBag.PageCount = (int)Math.Ceiling((double)totalNewsCount / pageSize);


            (int TotalArticles, int TotalPublishedArticles, int TotalViews, int ThisMonth) = await _newsService.GetAuthorsNewsSummaryAsync(user.Id);

            ViewBag.TotalArticles = TotalArticles;
            ViewBag.TotalPublishedArticles = (int)(((double)TotalPublishedArticles / (double)TotalArticles) * 100);
            ViewBag.TotalViews = TotalViews;
            ViewBag.ThisMonth = ThisMonth;

            return View(authorNewsAllFiltersDTO);
        }




        [HttpGet]
        public IActionResult AuthorNewsList(NewsFilterParametersDTO authorNewsFilterParametersDTO, string sortBy = "PublishedDate", SortTypes sortOptions = SortTypes.Default, int pageNo = 1, int pageSize = 5)
        {
            return ViewComponent("AuthorNews", new
            {
                authorNewsFilterParametersDTO,
                sortBy,
                sortOptions,
                pageNo,
                pageSize
            });
        }




        [HttpGet]
        public async Task<IActionResult> DetailedNews(Guid newsId)
        {
            AuthorsNewsDetailesToShowDTO authorsNewsDetailedDTO = await _newsService.GetAuthorsNewsDetailsAsync(newsId);
            string? videoLink = authorsNewsDetailedDTO.VideoUrl ?? "";
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
            authorsNewsDetailedDTO.VideoUrl = youtubeEmbedUrl;
            return View(authorsNewsDetailedDTO);
        }



        [HttpPost]
        public async Task<IActionResult> AddAdmin(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, "Admin");

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return BadRequest("User is already an Admin.");
                }
                if (result.Succeeded)
                {
                    return Ok($"User with email {email} has been added as Admin.");
                }
                else
                {
                    return BadRequest("Failed to add user to Admin role.");
                }
            }
            else
            {
                return NotFound($"User with email {email} not found.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> RemoveAdmin(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return BadRequest("User is not an Admin.");
                }
                var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    return Ok($"User with email {email} has been removed from Admin role.");
                }
                else
                {
                    return BadRequest("Failed to remove user from Admin role.");
                }
            }
            else
            {
                return NotFound($"User with email {email} not found.");
            }

        }
    }
}
