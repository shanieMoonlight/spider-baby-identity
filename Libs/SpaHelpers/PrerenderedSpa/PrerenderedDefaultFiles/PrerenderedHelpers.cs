using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace PrerenderedSpa.PrerenderedDefaultFiles;
internal static class PrerenderedHelpers
{

    internal static bool IsGetOrHeadMethod(string method) => 
        HttpMethods.IsGet(method) || HttpMethods.IsHead(method);

    //------------------// 

    internal static bool PathEndsInSlash(PathString path) => 
        path.HasValue && path.Value!.EndsWith('/');

    //------------------// 

    internal static string GetPathValueWithSlash(PathString path) => 
        PathEndsInSlash(path) ? path.Value! : path.Value + "/";

    //------------------// 

    internal static void RedirectToPathWithSlash(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
        var request = context.Request;
        var redirect = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path + "/", request.QueryString);
        context.Response.Headers.Location = redirect;

    }

}//Cls