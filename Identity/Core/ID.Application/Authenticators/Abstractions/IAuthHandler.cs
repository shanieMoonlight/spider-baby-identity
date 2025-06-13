using Microsoft.AspNetCore.Http;

namespace ID.Application.Authenticators.Abstractions;


/// <summary>
/// Interface for handling authorization.
/// </summary>
public interface IAuthHandler
{
    /// <summary>
    /// Determines whether the specified HTTP context is authorized.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns><c>true</c> if the specified context is authorized; otherwise, <c>false</c>.</returns>
    bool IsAuthorized(HttpContext context);
}



