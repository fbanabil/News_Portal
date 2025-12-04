using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;

namespace News_Portal.UI.ViewComponents
{
    public class OtherNewsViewComponent : ViewComponent
    {
        private readonly INewsService _newsService;
        public OtherNewsViewComponent(INewsService newsService)
        {
            _newsService = newsService;
        }
        public async Task<IViewComponentResult> InvokeAsync(NewsType newsType)
        {
            List<HomePageNewsToShowDTO> homePageNewsToShowDTOs = await _newsService.GetOtherNewsByTypeAsync(newsType);
            return View("OtherNewsViewComponent", homePageNewsToShowDTOs);
        }
    }
}
