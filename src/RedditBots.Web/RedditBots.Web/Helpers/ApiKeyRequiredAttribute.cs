using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RedditBots.Web.Helpers;

public class ApiKeyRequiredAttribute : ActionFilterAttribute
{
    public const string API_KEY_HEADER_NAME = "X-APIKEY";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var appSettings = context.HttpContext.RequestServices.GetRequiredService<IOptions<AppSettings>>().Value;

        context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var apiKeyHeaderValues);
        var requestApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (requestApiKey == null || appSettings.ApiKey != requestApiKey)
        {
            context.Result = new UnauthorizedObjectResult("Invalid ApiKey");
        }
    }
}
