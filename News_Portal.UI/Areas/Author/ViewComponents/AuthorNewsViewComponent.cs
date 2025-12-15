using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;

namespace News_Portal.UI.Areas.Author.ViewComponents
{
    public class AuthorNewsViewComponent: ViewComponent
    {
        private readonly INewsService _newsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorNewsViewComponent(INewsService newsService, UserManager<ApplicationUser> userManager)
        {
            _newsService = newsService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(AuthorNewsFilterParametersDTO authorNewsFilterParametersDTO, string sortBy="", SortTypes sortOptions = SortTypes.Default, int pageNo=1, int pageSize=10)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            List<AuthorsNewsToShowDTO> authorsNewsToShowDTOs = await _newsService.GetAllAuthorsNewsAsync(user.Id, authorNewsFilterParametersDTO, sortBy, sortOptions,pageNo,pageSize);
            return View("AuthorNewsViewComponent", authorsNewsToShowDTOs);
        }
    }
}
