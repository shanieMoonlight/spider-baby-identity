using Microsoft.AspNetCore.Authentication;

namespace MyIdDemo.Middleware.Cookies;

public static  class CookieDebugging
{
    /// <summary>
    /// Adds middleware to inspect authentication cookie details at a specified endpoint
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <param name="path">The endpoint path to expose cookie debug information (defaults to "/debug-cookie")</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseCookieDebugger(
        this IApplicationBuilder app,
        string path = "/debug-cookie")
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.Use(async (context, next) =>
        {
            if (context.Request.Path == path)
            {
                var authResult = await context.AuthenticateAsync();
                if (authResult?.Properties != null)
                {
                    await context.Response.WriteAsJsonAsync(new
                    {
                        context.User.Identity?.IsAuthenticated,
                        authResult.Properties.ExpiresUtc,
                        IsSlidingEnabled = authResult.Properties.AllowRefresh // This indicates sliding
                    });
                    return;
                }

                // Handle case when not authenticated
                await context.Response.WriteAsJsonAsync(new
                {
                    IsAuthenticated = false,
                    Message = "No authentication cookie found"
                });
                return;
            }

            await next();
        });
    }
}

