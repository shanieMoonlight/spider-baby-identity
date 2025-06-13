using ID.Application.AppAbs.FromApp;
using ID.Application.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ID.Application.Middleware;

//#####################################################//

/// <summary>
/// Middleware to detect requests from mobile applications based on HTTP headers.
/// </summary>
internal class FromAppMiddleware(RequestDelegate next, IIsFromMobileApp fromAppService, IOptions<IdApplicationOptions> options)
{
    private readonly IdApplicationOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        var isFromApp = context.Request.Headers
              .Where(header => header.Key == _options.FromAppHeaderKey)
              .Any(header => header.Value == _options.FromAppHeaderValue);

        fromAppService.IsFromApp = isFromApp;

        await next(context);
    }
}

//#####################################################//


// Extension method to make it easy to add the middleware to the pipeline
internal static class FromAppMiddlewareExtensions
{
    public static IApplicationBuilder UseFromAppMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<FromAppMiddleware>();
}


//#####################################################//