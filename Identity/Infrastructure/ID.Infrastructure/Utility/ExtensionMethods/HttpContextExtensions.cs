using Microsoft.AspNetCore.Http;

namespace ID.Infrastructure.Utility.ExtensionMethods;

internal static class HttpContextExtensions
{

    /// <summary>
    /// Checks whether this request came from a certain website page/request.
    /// Looks for the HangFire url at the start of the request
    /// </summary>
    internal static bool IsFromSection(this HttpContext ctx, string sectionUrl) => 
         ctx.Request.IsFromSection(sectionUrl);

    //--------------------------------//

    /// <summary>
    /// Checks whether the Query Param <paramref name="queryKey"/> is in the Request of the <paramref name="ctx"/>
    /// </summary>
    internal static bool ContainsParam(this HttpContext ctx, string queryKey) =>
        ctx.Request.ContainsParam(queryKey);


}//Cls

