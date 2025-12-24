using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Comment;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;
using News_Portal.UI.Filters;
using News_Portal.UI.Samples;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;


namespace News_Portal.UI.Controllers
{
    [Route("[controller]/[action]")]
    [TypeFilter(typeof(ModelStateValidationFilter))]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INewsService _newsService;
        private readonly ICommentService _commentService;
        private readonly IUpdateSample _updateSample;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, INewsService newsService, IUpdateSample updateSample, UserManager<ApplicationUser> userManager, ICommentService commentService)
        {
            _logger = logger;
            _newsService = newsService;
            _updateSample = updateSample;
            _userManager = userManager;
            _commentService = commentService;
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
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.PresentUserName = user?.PersonName ?? "Unknown";
            string videoLink = detailedNewsToShowDTOs.VideoUrl ?? "";
            string youtubeEmbedUrl="";
            if (videoLink.Contains("youtube.com/watch?v="))
            {

                var parts = videoLink.Split("watch?v=", StringSplitOptions.RemoveEmptyEntries);
                var idPart = parts.Last().Split('&')[0];
                youtubeEmbedUrl = $"https://www.youtube.com/embed/{idPart}";
            }
            else if (videoLink.Contains("youtu.be/"))
            {
                var parts = videoLink.Split("youtu.be/", StringSplitOptions.RemoveEmptyEntries);
                var idPart = parts.Last().Split('?')[0];
                youtubeEmbedUrl = $"https://www.youtube.com/embed/{idPart}";
            }
            ViewBag.VideoLink = youtubeEmbedUrl;
            detailedNewsToShowDTOs.VideoUrl = youtubeEmbedUrl;

            ViewBag.CurrentUserId = user?.Id;
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
                pageSize = 12;
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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CommentToAddDTO commentToAddDTO)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(HttpContext.User);
            CommentToShowDTO commentToShowDTO = await _commentService.AddCommentAsync(commentToAddDTO,user.Id);
            return PartialView("~/Views/Shared/PartialViews/_Comment.cshtml",commentToShowDTO);
        }

        //[Route("Home/Error")]
        [HttpGet]
        public IActionResult Error()
        {
            // Try to read the exception (may be null when middleware redirected)
            var exFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var errorMessage = exFeature?.Error?.Message;
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            Response.StatusCode = StatusCodes.Status500InternalServerError;
            ViewData["ErrorMessage"] = errorMessage ?? "An unexpected error occurred.";
            ViewData["RequestId"] = requestId;

            return View();
        }
    }
}
