using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize(Roles = "Author,Admin")]
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
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] AuthorNewsFilterParametersDTO? parametersDTO,
                                      [FromQuery] string? sortBy = "PublishedDate",
                                      [FromQuery] SortTypes sortType = SortTypes.Default,
                                      [FromQuery] int pageNo = 1,
                                      [FromQuery] int pageSize = 5)
        {
            AuthorNewsAllFiltersDTO authorNewsAllFiltersDTO = new AuthorNewsAllFiltersDTO();
            authorNewsAllFiltersDTO.parametersDTO = parametersDTO ?? new AuthorNewsFilterParametersDTO();
            authorNewsAllFiltersDTO.sortBy = sortBy ?? "PublishedDate";
            authorNewsAllFiltersDTO.sortType = sortType;
            authorNewsAllFiltersDTO.pageNo = pageNo;
            authorNewsAllFiltersDTO.pageSize = pageSize;

            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            List<AuthorsNewsToShowDTO> authorsNewsToShowDTOs = await _newsService.GetAllAuthorsNewsAsync(user.Id, parametersDTO, sortBy, sortType, pageNo, pageSize);
            int totalNewsCount = await _newsService.GetAuthorsNewsCountAsync(user.Id, parametersDTO);
            ViewBag.PageCount = (int)Math.Ceiling((double)totalNewsCount / pageSize);
            return View(authorNewsAllFiltersDTO);
        }

        [HttpGet]
        public IActionResult AuthorNewsList(AuthorNewsFilterParametersDTO authorNewsFilterParametersDTO,
                                   string sortBy = "PublishedDate",
                                   SortTypes sortOptions = SortTypes.Default,
                                   int pageNo = 1,
                                   int pageSize = 5)
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

    }
}
