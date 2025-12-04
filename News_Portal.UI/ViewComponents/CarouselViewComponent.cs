using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.DTO.News;
using News_Portal.Core.ServiceContracts;

namespace News_Portal.UI.ViewComponents
{
    public class CarouselViewComponent : ViewComponent
    {
        private readonly INewsService _newsService;
        public CarouselViewComponent(INewsService newsService)
        {
            _newsService = newsService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<HomePageNewsToShowDTO> homePageNewsToShowDTOs = await _newsService.GetNewsForHomePageCarouselAsync();
            return View("CarouselViewComponent",homePageNewsToShowDTOs);
        }
    }
}
