using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;
using News_Portal.UI.Filters;

namespace News_Portal.UI.Areas.Author.Controllers
{
    [Area("Author")]
    [Route("[area]/[controller]/[action]")]
    [TypeFilter(typeof(ModelStateValidationFilter))]
    [Authorize(Roles = "Author")]
    public class AuthorActionsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INewsService _newsService;



        public AuthorActionsController(UserManager<ApplicationUser> userManager, INewsService newsService)
        {
            _userManager = userManager;
            _newsService = newsService;
        }




        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] AuthorNewsFilterParametersDTO? parametersDTO, [FromQuery] string? sortBy = "PublishedDate", [FromQuery] SortTypes sortType = SortTypes.Default, [FromQuery] int pageNo = 1, [FromQuery] int pageSize = 5)
        {
            AuthorNewsAllFiltersDTO authorNewsAllFiltersDTO = new AuthorNewsAllFiltersDTO();
            authorNewsAllFiltersDTO.parametersDTO = parametersDTO ?? new AuthorNewsFilterParametersDTO();
            authorNewsAllFiltersDTO.sortBy = sortBy ?? "PublishedDate";
            authorNewsAllFiltersDTO.sortType = sortType;
            authorNewsAllFiltersDTO.pageNo = 1;
            authorNewsAllFiltersDTO.pageSize = pageSize;

            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);

            List<AuthorsNewsToShowDTO> authorsNewsToShowDTOs = await _newsService.GetAllAuthorsNewsAsync(user.Id, parametersDTO, sortBy, sortType, pageNo, pageSize);

            int totalNewsCount = await _newsService.GetAuthorsNewsCountAsync(user.Id, parametersDTO);
            ViewBag.PageCount = (int)Math.Ceiling((double)totalNewsCount / pageSize);


            ( int TotalArticles, int TotalPublishedArticles, int TotalViews, int ThisMonth ) = await _newsService.GetAuthorsNewsSummaryAsync(user.Id);

            ViewBag.TotalArticles = TotalArticles;
            ViewBag.TotalPublishedArticles = (int)(((double)TotalPublishedArticles/(double)TotalArticles)*100);
            ViewBag.TotalViews = TotalViews;
            ViewBag.ThisMonth = ThisMonth;

            return View(authorNewsAllFiltersDTO);
        }






        [HttpGet]
        public IActionResult AuthorNewsList(AuthorNewsFilterParametersDTO authorNewsFilterParametersDTO, string sortBy = "PublishedDate", SortTypes sortOptions = SortTypes.Default, int pageNo = 1, int pageSize = 5)
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






        [HttpGet]
        public async Task<IActionResult> AddNews()
        {
            return View();
        }






        [HttpPost]
        public async Task<IActionResult> AddNews(NewsToAddDTO newsToAddDTO)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            await _newsService.AddNewsByAuthor(newsToAddDTO, user!.Id);
            return RedirectToAction(nameof(Index));
        }





        [HttpGet]
        public async Task<IActionResult> EditNews(Guid newsId)
        {
            NewsToEditDTO newsToEditDTO = await _newsService.GetNewsForEditAsync(newsId);
            return View(newsToEditDTO);

        }




        [HttpPost]
        public async Task<IActionResult> EditNews(NewsToEditDTO newsToEditDTO)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            await _newsService.UpdateNewsAsync(newsToEditDTO, user!.Id);
            return RedirectToAction(nameof(DetailedNews), new { newsId = newsToEditDTO.NewsId });
        }





        [HttpGet]
        public async Task<IActionResult> DeleteNews(Guid newsId)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            await _newsService.DeleteNewsAsync(newsId, user!.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
