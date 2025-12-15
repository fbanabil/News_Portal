using Microsoft.AspNetCore.Mvc;

namespace News_Portal.UI.Areas.Author.ViewComponents
{
    public class AuthorsBestViewComponent : ViewComponent
    {
        public AuthorsBestViewComponent()
        {
        }
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
