using ID.OAuth.Facebook.Data;
using MyResults;

namespace ID.OAuth.Facebook.Services;

public interface IFacebookTokenVerifier
{
    Task<GenResult<FacebookUserProfile>> GetUserProfileAsync(string userAccessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify a Facebook user access token server-side using the Graph debug_token endpoint.
    /// Returns a result object indicating validity, the Facebook user id and scopes.
    /// </summary>
    Task<GenResult<FacebookDebugTokenData>> VerifyTokenAsync(string authToken, string expectedUserId, CancellationToken cancellationToken = default);
}
