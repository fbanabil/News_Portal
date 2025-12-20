using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.News;
using News_Portal.Core.ServiceContracts;

namespace News_Portal.UI.Areas.Author.ViewComponents
{
    [Authorize(Roles = "Author")]
    public class AuthorsBestViewComponent : ViewComponent
    {
        private readonly INewsService _newsService;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthorsBestViewComponent(INewsService newsService, UserManager<ApplicationUser> userManager)
        {
            _newsService = newsService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(int size = 4)
        {
            ApplicationUser? applicationUser = await _userManager.GetUserAsync(HttpContext.User);
            List<HomePageNewsToShowDTO> bestOfAuthorsNewsDTO = await _newsService.GetAuthorsTopNewsAsync(size, applicationUser!.Id);

            bestOfAuthorsNewsDTO = bestOfAuthorsNewsDTO.Select(b=>
            {
                if(b?.TotalViews==null) 
                {
                    b.TotalViews = 0;
                }
                return b;
            }).ToList();

            return View("AuthorsBestViewComponent", bestOfAuthorsNewsDTO);
        }
    }
}
