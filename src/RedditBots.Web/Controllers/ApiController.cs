using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RedditBots.Web.Controllers
{
    [ApiController]
    [Route("Api")]
    public class ApiController : ControllerBase
    {
        [HttpPost("Log")]
        public IActionResult Index()
        {
            Console.WriteLine("LOGGED");

            return Ok();
        }
    }
}