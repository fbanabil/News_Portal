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
            
            
            if(newsType == NewsType.Sports || newsType==NewsType.Technology )
            {
                return View("OtherNewsViewComponent2", homePageNewsToShowDTOs);
            }
            else if(newsType == NewsType.Health || newsType == NewsType.Science)
            {
                return View("OtherNewsViewComponent3", homePageNewsToShowDTOs);
            }
                // take first 4 news items for other news types


                homePageNewsToShowDTOs = homePageNewsToShowDTOs.Take(4).ToList();

            return View("OtherNewsViewComponent", homePageNewsToShowDTOs);
        }
    }
}
