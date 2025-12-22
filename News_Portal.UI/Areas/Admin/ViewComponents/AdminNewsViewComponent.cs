using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;

namespace News_Portal.UI.Areas.Admin.ViewComponents
{
    public class AdminNewsViewComponent: ViewComponent
    {
        private readonly INewsService _newsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminNewsViewComponent(INewsService newsService, UserManager<ApplicationUser> userManager)
        {
            _newsService = newsService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(NewsFilterParametersDTO adminNewsFilterParametersDTO, string? authorEmail, string sortBy="", SortTypes sortOptions = SortTypes.Default, int pageNo=1, int pageSize=10)
        {

            List<AuthorsNewsToShowDTO> authorsNewsToShowDTOs = new List<AuthorsNewsToShowDTO>();

            if(authorEmail != null)
            {
                ApplicationUser? author = await _userManager.FindByEmailAsync(authorEmail);

                if (author != null)
                {
                    authorsNewsToShowDTOs = await _newsService.GetAllAuthorsNewsAsync(author.Id, adminNewsFilterParametersDTO, sortBy, sortOptions, pageNo, pageSize);
                }
                else
                {
                    authorsNewsToShowDTOs = await _newsService.GetAllNewsAsync(adminNewsFilterParametersDTO, sortBy, sortOptions, pageNo, pageSize);
                }
            }
            else
            {
                authorsNewsToShowDTOs = await _newsService.GetAllNewsAsync(adminNewsFilterParametersDTO, sortBy, sortOptions, pageNo, pageSize);
            }

            return View("AdminNewsViewComponent", authorsNewsToShowDTOs);
        }
    }
}
