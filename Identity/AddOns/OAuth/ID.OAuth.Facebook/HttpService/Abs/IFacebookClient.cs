using ID.OAuth.Facebook.Data;
using MyResults;

namespace ID.OAuth.Facebook.HttpService.Abs;

internal interface IFacebookHttpClient
{
    Task<GenResult<FacebookDebugTokenData>> GetDebugTokenAsync(string userAccessToken, CancellationToken cancellationToken = default);
    Task<GenResult<FacebookUserProfile>> GetUserProfileAsync(string userAccessToken, CancellationToken cancellationToken = default);
}