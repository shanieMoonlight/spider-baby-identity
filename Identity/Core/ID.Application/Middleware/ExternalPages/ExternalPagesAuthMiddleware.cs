using ID.Application.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;


namespace ID.Application.Middleware.ExternalPages;



//#############################//

public static class ExternalPagesAuthConstants
{
    //public const string ExternalPagesStartPath = "/ExternalPages";
    public const string WWWAuthenticateHeader = "Bearer";
    public const string ResponseContentType = "application/json";
    public static object ErrorResponse(string page) => new { Error = $"Unauthorized: {page} access requires authentication." };
    public static readonly Predicate<HttpContext> DefaultAuthPredicate = (context) => false; // By default, no access
}

//#############################//


public record ExternalPagesAuthMiddlewareOptions(Predicate<HttpContext> ExternalPagesAuthPredicate, string ExternalPagesStartPath);


//#############################//

public class ExternalPagesAuthMiddleware(RequestDelegate next, IOptions<ExternalPagesAuthMiddlewareOptions> iOptsProvider)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var options = iOptsProvider.Value;
        Predicate<HttpContext> authPredicate = options.ExternalPagesAuthPredicate ?? ExternalPagesAuthConstants.DefaultAuthPredicate;
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

        context.Response.Headers.WWWAuthenticate = ExternalPagesAuthConstants.WWWAuthenticateHeader;
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = ExternalPagesAuthConstants.ResponseContentType;
        await context.Response.WriteAsJsonAsync(ExternalPagesAuthConstants.ErrorResponse(options.ExternalPagesStartPath));
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


    public static IApplicationBuilder UseExternalPagesAuth_MntcMinimum(
        this IApplicationBuilder builder, string externalPagesPathStart) =>
        builder.UseExternalPagesAuth_Custom(externalPagesPathStart, ctx => ctx.IsInMntcTeamMinimum());

    public static IApplicationBuilder UseExternalPagesAuth_CustomerMinimum(
        this IApplicationBuilder builder, string externalPagesPathStart) =>
        builder.UseExternalPagesAuth_Custom(externalPagesPathStart, ctx => ctx.IsInCustomerTeamMinimum());


    public static IApplicationBuilder UseExternalPagesAuth_Mntc(
        this IApplicationBuilder builder, string externalPagesPathStart) =>
        builder.UseExternalPagesAuth_Custom(externalPagesPathStart, ctx => ctx.IsInMntcTeam());

    public static IApplicationBuilder UseExternalPagesAuth_Customer(
        this IApplicationBuilder builder, string externalPagesPathStart) =>
        builder.UseExternalPagesAuth_Custom(externalPagesPathStart, ctx => ctx.IsInCustomerTeam());



}//Cls

//#############################//