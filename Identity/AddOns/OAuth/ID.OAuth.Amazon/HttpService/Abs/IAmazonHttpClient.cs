using MyResults;
using ID.OAuth.Amazon.Data;

namespace ID.OAuth.Amazon.HttpService.Abs;

public interface IAmazonHttpClient
{
    Task<GenResult<AmazonTokenInfo>> GetTokenInfoAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<GenResult<AmazonUserProfile>> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken = default);
}
