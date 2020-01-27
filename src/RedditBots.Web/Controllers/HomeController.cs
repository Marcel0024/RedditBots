using Microsoft.AspNetCore.Mvc;
using RedditBots.Web.Helpers;

namespace RedditBots.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly LogsHelper _logsHelper;

        public HomeController(LogsHelper logsHelper)
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