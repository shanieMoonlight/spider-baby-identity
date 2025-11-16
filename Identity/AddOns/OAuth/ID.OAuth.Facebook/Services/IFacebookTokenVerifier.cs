

namespace ID.OAuth.Facebook.Services;

public interface IFacebookTokenVerifier
{
    /// <summary>
    /// Verify a Facebook user access token server-side using the Graph debug_token endpoint.
    /// Returns a result object indicating validity, the Facebook user id and scopes.
    /// </summary>
    Task<FacebookTokenVerificationResult> VerifyTokenAsync(string userAccessToken, CancellationToken cancellationToken = default);
}
