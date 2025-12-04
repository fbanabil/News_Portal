using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.DTO.News;
using News_Portal.Core.ServiceContracts;

namespace News_Portal.UI.ViewComponents
{
    public class TopNewsViewComponent : ViewComponent
    {
        private readonly INewsService _newsService;
        public TopNewsViewComponent(INewsService newsService)
        {
            _newsService = newsService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<HomePageNewsToShowDTO> topNews = await _newsService.GetNewsForHomePageCarouselAsync();
            return View("TopNewsViewComponent", topNews);
        }
    }
}
