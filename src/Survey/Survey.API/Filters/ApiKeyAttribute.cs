using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Survey.API.Filters
{
    public class ApiKeyAttribute : ActionFilterAttribute
    {
        private const string API_KEY_HEADER = "X-Api-Key";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key is missing.");
                return;
            }
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var configuredApiKey = Environment.GetEnvironmentVariable("API_KEY")
                                   ?? configuration["API_KEY"];

            if (configuredApiKey is not null && !configuredApiKey.Equals(extractedApiKey))
            {
                context.Result = new ForbidResult(); // 403 Forbidden
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
