using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;


namespace ID.Application.Middleware.ExternalPages;


/// <summary>
/// Install Swagger stuff
/// </summary>
public static class SwaggerAuthMiddlewareExtensions
{

    public const string SwaggerStartPath = "/swagger";
    /// <summary>
    /// Make sure Swagger is authenticated
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="swaggerAuthPredicate">Predicate to determine if Swagger authentication is required</param>
    /// <param name="swaggerStartPath">Path segment to identify Swagger Request. Default = "/swagger"</param>
    public static IApplicationBuilder UseSwaggerAuth(
        this IApplicationBuilder builder,
        Predicate<HttpContext> swaggerAuthPredicate,
        string swaggerStartPath = SwaggerStartPath) =>
        builder.UseExternalPagesAuth_Custom(swaggerStartPath, swaggerAuthPredicate);

    public static IApplicationBuilder UseSwaggerAuth_SuperTeam(
        this IApplicationBuilder builder, string swaggerStartPath = SwaggerStartPath) =>
        builder.UseExternalPagesAuth_SuperTeam(swaggerStartPath);


    public static IApplicationBuilder UseSwaggerAuth_MntcTeam(
        this IApplicationBuilder builder, string swaggerStartPath = SwaggerStartPath) =>
        builder.UseExternalPagesAuth_MntcTeam(swaggerStartPath);

    public static IApplicationBuilder UseSwaggerAuth_MntcMinimum(
        this IApplicationBuilder builder, string swaggerStartPath = SwaggerStartPath) =>
        builder.UseExternalPagesAuth_MntcMinimum(swaggerStartPath);


    public static IApplicationBuilder UseSwaggerAuth_CustomerMinimum(
        this IApplicationBuilder builder, string swaggerStartPath = SwaggerStartPath) =>
        builder.UseExternalPagesAuth_CustomerMinimum(swaggerStartPath);

    public static IApplicationBuilder UseSwaggerAuth_CustomerTeam(
        this IApplicationBuilder builder, string swaggerStartPath = SwaggerStartPath) =>
        builder.UseExternalPagesAuth_CustomerTeam(swaggerStartPath);


}//Cls
