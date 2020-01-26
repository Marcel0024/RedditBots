using Microsoft.AspNetCore.Mvc;

namespace RedditBots.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}