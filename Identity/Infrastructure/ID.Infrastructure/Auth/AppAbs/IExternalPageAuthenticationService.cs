using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ID.Infrastructure.Auth.AppAbs;


/// <summary>
/// Service for handling authentication for external pages.
/// </summary>
public interface IExternalPageAuthenticationService
{
    /// <summary>
    /// Checks if the request is from an external page.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>True if the request is from an external page, otherwise false.</returns>
    Task<AuthenticateResult> TryAuthenticateAsync(HttpContext context);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Authenticates the user based on the context.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The authentication result.</returns>

    bool IsFromExternalPage(HttpRequest request);

}//int