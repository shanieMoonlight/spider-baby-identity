using ID.Application.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;


namespace ID.Application.Middleware.ExternalPages;



//#############################//


public record ExternalPagesAuthMiddlewareOptions(Predicate<HttpContext> ExternalPagesAuthPredicate, string ExternalPagesStartPath);


//#############################//

public class ExternalPagesAuthMiddleware(RequestDelegate next, IOptions<ExternalPagesAuthMiddlewareOptions> iOptsProvider)
{
    internal const string _wwwAuthenticateHeader = "Bearer";
    internal const string _responseContentType = "application/json";
    internal static object ErrorResponse(string page) => new { Error = $"Unauthorized: {page} access requires authentication." };
    internal static readonly Predicate<HttpContext> _defaultAuthPredicate = (context) => false; // By default, no access

    //-----------------------------//

    public async Task InvokeAsync(HttpContext context)
    {
        var options = iOptsProvider.Value;
        Predicate<HttpContext> authPredicate = options.ExternalPagesAuthPredicate ?? _defaultAuthPredicate;
        var ExternalPagesStartPathString = GetExternalPagesStartSegment(options.ExternalPagesStartPath);


        //Don't block non-ExternalPages
        if (!context.Request.Path.StartsWithSegments(ExternalPagesStartPathString))
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

        context.Response.Headers.WWWAuthenticate = _wwwAuthenticateHeader;
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = _responseContentType;
        await context.Response.WriteAsJsonAsync(ErrorResponse(options.ExternalPagesStartPath));
    }



    private static PathString GetExternalPagesStartSegment(string pathStart) =>
        !pathStart.StartsWith('/')
            ? new PathString($"/{pathStart}")
            : new PathString(pathStart);

}//Cls


//#############################//

/// <summary>
/// Install ExternalPages stuff
/// </summary>
public static class ExternalPagesAuthMiddlewareExtensions
{

    /// <summary>
    /// Make sure ExternalPages is authenticated
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="externalPagesAuthPredicate">Predicate to determine if ExternalPages authentication is required</param>
    /// <param name="externalPagesPathStart">Path segment to identify ExternalPages Request. Default = "/ExternalPages"</param>
    public static IApplicationBuilder UseExternalPagesAuth_Custom(
        this IApplicationBuilder builder, string externalPagesPathStart, Predicate<HttpContext> externalPagesAuthPredicate)
    {
        var authOptions = new ExternalPagesAuthMiddlewareOptions(externalPagesAuthPredicate, externalPagesPathStart);
        builder.UseMiddleware<ExternalPagesAuthMiddleware>(Options.Create(authOptions));
        return builder;
    }

    public static IApplicationBuilder UseExternalPagesAuth_SuperTeam(
        this IApplicationBuilder builder, string externalPagesPathStart) =>
        builder.UseExternalPagesAuth_Custom(externalPagesPathStart, ctx => ctx.IsInSuperTeam());

    public static IApplicationBuilder UseExternalPagesAuth_MntcTeam(
        this IApplicationBuilder builder, string externalPagesPathStart) =>
        builder.UseExternalPagesAuth_Custom(externalPagesPathStart, ctx => ctx.IsInMntcTeam());


    public static IApplicationBuilder UseExternalPagesAuth_MntcMinimum(
        this IApplicationBuilder builder, string externalPagesPathStart) =>
        builder.UseExternalPagesAuth_Custom(externalPagesPathStart, ctx => ctx.IsInMntcTeamMinimum());


    public static IApplicationBuilder UseExternalPagesAuth_CustomerTeam(
        this IApplicationBuilder builder, string externalPagesPathStart) =>
        builder.UseExternalPagesAuth_Custom(externalPagesPathStart, ctx => ctx.IsInCustomerTeam());

    public static IApplicationBuilder UseExternalPagesAuth_CustomerMinimum(
        this IApplicationBuilder builder, string externalPagesPathStart) =>
        builder.UseExternalPagesAuth_Custom(externalPagesPathStart, ctx => ctx.IsInCustomerTeamMinimum());



}//Cls


//#############################//