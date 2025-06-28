using ID.Application.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;


namespace ID.AddOns.Middleware.Swagger;



//#############################//

public static class SwaggerAuthConstants
{
    public const string SwaggerStartPath = "/swagger";
    public const string WWWAuthenticateHeader = "Bearer";
    public const string ResponseContentType = "application/json";
    public static readonly object ErrorResponse = new { Error = "Unauthorized: Swagger access requires authentication." };
    public static readonly Predicate<HttpContext> DefaultAuthPredicate = (context) => true;
}

//#############################//


public record SwaggerAuthMiddlewareOptions(
    Predicate<HttpContext>? SwaggerAuthPredicate,
    string? SwaggerStartPath);


//#############################//

public class SwaggerAuthMiddleware(RequestDelegate next, IOptions<SwaggerAuthMiddlewareOptions> iOptsProvider)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var options = iOptsProvider.Value;
        Predicate<HttpContext> authPredicate = options.SwaggerAuthPredicate ?? SwaggerAuthConstants.DefaultAuthPredicate;
        var swaggerStartPath = GetSwaggerStartSegment(options);


        //Don't block non-swagger
        if (!context.Request.Path.StartsWithSegments(swaggerStartPath))
        {
            await next.Invoke(context).ConfigureAwait(false);
            return;
        }


        //Allow authenticated
        if (
            context.User.Identity is not null
            && context.User.Identity.IsAuthenticated
            && authPredicate(context))
        {
            await next.Invoke(context).ConfigureAwait(false);
            return;
        }

        context.Response.Headers.WWWAuthenticate = SwaggerAuthConstants.WWWAuthenticateHeader;
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = SwaggerAuthConstants.ResponseContentType;
        await context.Response.WriteAsJsonAsync(SwaggerAuthConstants.ErrorResponse);
    }



    private static PathString GetSwaggerStartSegment(SwaggerAuthMiddlewareOptions options)
    {
        var swaggerStartPath = string.IsNullOrWhiteSpace(options.SwaggerStartPath)
           ? SwaggerAuthConstants.SwaggerStartPath
           : options.SwaggerStartPath;

        if (!swaggerStartPath.StartsWith('/'))
            swaggerStartPath = $"/{swaggerStartPath}";

        return new PathString(swaggerStartPath);
    }

}//Cls


//#############################//

/// <summary>
/// Install Swagger stuff
/// </summary>
public static class SwaggerAuthMiddlewareExtensions
{

    /// <summary>
    /// Make sure Swagger is authenticated
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="swaggerAuthPredicate">Predicate to determine if Swagger authentication is required</param>
    /// <param name="swaggerStartPath">Path segment to identify Swagger Request. Default = "/swagger"</param>
    public static IApplicationBuilder UseSwaggerAuth(
        this IApplicationBuilder builder,
        Predicate<HttpContext>? swaggerAuthPredicate = null,
        string? swaggerStartPath = null
    )
    {
        var authOptions = new SwaggerAuthMiddlewareOptions(swaggerAuthPredicate, swaggerStartPath);
        builder.UseMiddleware<SwaggerAuthMiddleware>(Options.Create(authOptions));
        return builder;
    }


    public static IApplicationBuilder UseSwaggerAuthSuperTeam(
        this IApplicationBuilder builder, string? swaggerStartPath = null) =>
        builder.UseSwaggerAuth(ctx => ctx.IsInSuperTeam(), swaggerStartPath);


    public static IApplicationBuilder UseSwaggerAuthMntcMinimum(
        this IApplicationBuilder builder, string? swaggerStartPath = null) =>
        builder.UseSwaggerAuth(ctx => ctx.IsInMntcTeamMinimum(), swaggerStartPath);

}//Cls

//#############################//