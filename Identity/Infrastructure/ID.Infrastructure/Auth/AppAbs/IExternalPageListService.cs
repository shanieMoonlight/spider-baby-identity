using Microsoft.AspNetCore.Http;

namespace ID.Infrastructure.Auth.AppAbs;


/// <summary>
/// Service for managing external pages.
/// </summary>
public interface IExternalPageListService
{
    /// <summary>
    /// Gets the list of external pages.
    /// </summary>
    /// <returns>List of external pages.</returns>
    List<string> GetExternalPages();

    //- - - - - - - - - - - - - - - - - - //


    /// <summary>
    /// Checks if the request is from an external page.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>True if the request is from an external page, otherwise false.</returns>
    bool IsFromExternalPage(HttpRequest request);
}