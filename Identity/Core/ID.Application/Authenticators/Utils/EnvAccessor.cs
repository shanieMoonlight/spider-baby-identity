using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ID.Application.Authenticators.Utils;

internal static class EnvAccessor
{
    public static IWebHostEnvironment GetEnv(this HttpContext ctx) =>
        ctx.RequestServices.GetRequiredService<IWebHostEnvironment>();

    public static bool IsDevEnv(this HttpContext ctx) =>
        ctx.GetEnv().IsDevelopment();

    public static bool IsProdEnv(this HttpContext ctx) =>
        ctx.GetEnv().IsProduction();
}


//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//
//#-#-#-#-#-#      SETUP     #-#-#-#-#-#//
//#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=#=//