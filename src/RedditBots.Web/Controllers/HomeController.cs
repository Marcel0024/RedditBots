using Microsoft.AspNetCore.Mvc;
using RedditBots.Web.Helpers;

namespace RedditBots.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly LogHandler _logsHelper;

        public HomeController(LogHandler logsHelper)
        {
            _logsHelper = logsHelper;
        }

        public IActionResult Index()
        {
            if (_logsHelper.LastLogDateTime.HasValue)
            {
                ViewData["LastTimeUpdated"] = _logsHelper.LastLogDateTime.Value.ToShortTimeString();
            }

            return View();
        }
    }
}