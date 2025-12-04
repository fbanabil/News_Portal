using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;

namespace News_Portal.UI.ViewComponents
{
    public class SuggestionViewComponent : ViewComponent
    {
        private readonly INewsService _newsService;
        public SuggestionViewComponent(INewsService newsService)
        {
            _newsService = newsService;
        }
        public async Task<IViewComponentResult> InvokeAsync(NewsType newsType)
        {
            List<HomePageNewsToShowDTO> suggestions = await _newsService.GetSuggestionNewsByTypeAsync(newsType);
            return View("SuggestionViewComponent", suggestions);
        }
    }
}
