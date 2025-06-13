using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ID.Infrastructure.Utility.ExtensionMethods;

internal static class MsgReceivedCtxExtensions
{
    /// <summary>
    /// Checks whether this request came from a certain website page/request.
    /// Looks for the <paramref name="sectionUrl"/> url at the start of the request
    /// </summary>
    internal static bool IsFromSection(this MessageReceivedContext mrCtx, string sectionUrl) =>
        mrCtx.HttpContext.IsFromSection(sectionUrl);

    //--------------------------------//

    /// <summary>
    /// Checks whether the Query Param is in the Request of the <paramref name="mrCtx"/>
    /// </summary>
    internal static bool ContainsParam(this MessageReceivedContext mrCtx, string queryKey) =>
        mrCtx.HttpContext.ContainsParam(queryKey);


}//Cls

