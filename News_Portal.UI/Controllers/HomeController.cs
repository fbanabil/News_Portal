using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;
using News_Portal.UI.Filters;
using News_Portal.UI.Samples;
using System.Diagnostics;
using System.Threading.Tasks;


namespace News_Portal.UI.Controllers
{
    [Route("[controller]/[action]")]
    [TypeFilter(typeof(ModelStateValidationFilter))]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INewsService _newsService;
        private readonly IUpdateSample _updateSample;

        public HomeController(ILogger<HomeController> logger, INewsService newsService, IUpdateSample updateSample)
        {
            _logger = logger;
            _newsService = newsService;
            _updateSample = updateSample;
        }

        [HttpGet]

        public async Task<IActionResult> Index()
        {
            //await _updateSample.UpdateAsync();
            return View();
        }

        [HttpGet("{newsId}")]
        public async Task<IActionResult> Details([FromRoute] Guid newsId)
        {
            DetailedNewsToShowDTO detailedNewsToShowDTOs = await _newsService.GetDetailedNewsToShowDTOsByNewsId(newsId);
            return View(detailedNewsToShowDTOs);
        }


        [HttpGet("{newsType}/{pageNo?}/{pageSize?}")]
        public async Task<IActionResult> GetNews([FromRoute] NewsType newsType,[FromRoute] int? pageNo, [FromRoute] int? pageSize)
        {
            if(pageNo == null || pageNo <= 0)
            {
                pageNo = 1;
            }

            if(pageSize == null || pageSize <= 0)
            {
                pageSize = 18;
            }
            int p=pageNo.Value,ps=pageSize.Value;

            List<HomePageNewsToShowDTO> NewsByType = await _newsService.GetNewsByTypeAsync(newsType,p,ps);
            ViewBag.NewsType = newsType;
            ViewBag.PageNo = p;
            ViewBag.PageSize = ps;

            int totalOfThisType = await _newsService.GetTotalNewsCountByTypeAsync(newsType);

            ViewBag.PageCount = (int)Math.Ceiling((double)totalOfThisType / ps);
            return View(NewsByType);
        }
    }
}
