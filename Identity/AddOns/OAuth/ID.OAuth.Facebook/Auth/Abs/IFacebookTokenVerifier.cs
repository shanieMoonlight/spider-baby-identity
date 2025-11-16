//using ID.OAuth.Facebook.Data;
//using MyResults;

//namespace ID.OAuth.Facebook.Auth.Abs;

///// <summary>
///// Service for verifying Facebook access tokens server-side.
///// Validates tokens with Facebook's Graph API and returns verified user data.
///// </summary>
//public interface IFacebookTokenVerifier
//{
//    /// <summary>
//    /// Verifies a Facebook access token and returns the verified user payload.
//    /// </summary>
//    /// <param name="accessToken">Facebook access token to verify</param>
//    /// <param name="cancellationToken">Cancellation token</param>
//    /// <returns>Result containing verified user data or error information</returns>
//    Task<GenResult<FacebookVerifiedPayload>> VerifyTokenAsync(string accessToken, CancellationToken cancellationToken);
//}
