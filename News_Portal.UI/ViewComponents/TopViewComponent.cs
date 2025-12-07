using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;

namespace News_Portal.UI.ViewComponents
{
    public class TopViewComponent : ViewComponent
    {
        private readonly INewsService _newsService;
        public TopViewComponent(INewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task<IViewComponentResult> InvokeAsync(TopOfXType type,int cnt)
        {
            List<HomePageNewsToShowDTO> topOfWeekNews = await _newsService.GetTopNewsAsync(type, cnt);
            return View("TopViewComponent", topOfWeekNews);
        }
    }
}
