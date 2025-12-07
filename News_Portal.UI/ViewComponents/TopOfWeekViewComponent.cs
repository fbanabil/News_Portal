using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.DTO.News;
using News_Portal.Core.ServiceContracts;

namespace News_Portal.UI.ViewComponents
{
    public class TopOfWeekViewComponent : ViewComponent
    {
        private readonly INewsService _newsService;
        public TopOfWeekViewComponent(INewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<HomePageNewsToShowDTO> topOfWeekNews = await _newsService.GetTopOfWeekNewsAsync();
            return View("TopOfWeekViewComponent", topOfWeekNews);
        }
    }
}
