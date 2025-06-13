using Google.Apis.Auth;
using ID.OAuth.Google.Data;
using MyResults;

namespace ID.OAuth.Google.Auth.Abs;
public interface IGoogleTokenVerifier
{
    Task<GenResult<GoogleVerifiedPayload>> VerifyTokenAsync(string token, CancellationToken cancellationToken);
}