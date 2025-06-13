using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace ID.Infrastructure.Utility.ExtensionMethods;

internal static class HttpRequestExtensions
{
    //--------------------------------//

    /// <summary>
    /// Checks whether this request came from a certain website page/request.
    /// Looks for the <paramref name="sectionUrl"/> at the start of the request
    /// </summary>
    internal static bool IsFromSection(this HttpRequest request, string sectionUrl)
    {
        // Look for HangFire/Swagger stuff
        var path = request.Path.HasValue ? request.Path.Value : "";
        var pathBase = request.PathBase.HasValue ? request.PathBase.Value : path;

        return path.StartsWith(sectionUrl) || pathBase.StartsWith(sectionUrl);
    }

    //--------------------------------//

    /// <summary>
    /// Checks whether the Query Param <paramref name="queryKey"/> is in the Request of the <paramref name="request"/>
    /// </summary>
    internal static bool ContainsParam(this HttpRequest request, string queryKey) =>
        request.Query.ContainsKey(queryKey);

    //--------------------------------//

    /// <summary>
    /// Checks whether the Query Param <paramref name="queryKey"/> is in the Request of the <paramref name="request"/>
    /// </summary>
    internal static string GetParamValue(this HttpRequest request, string queryKey)
    {
        var query = QueryHelpers.ParseQuery(request.QueryString.Value);
        query.TryGetValue(queryKey, out var token);
        return token.ToString();
    }

    //--------------------------------//


}//Cls

