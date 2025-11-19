using MyResults;
using ID.OAuth.Amazon.Data;

namespace ID.OAuth.Amazon.Services.Abs;

public interface IAmazonAuthenticationService
{
    Task<GenResult<AmazonTokenInfo>> VerifyTokenAsync(string accessToken, string expectedUserId, CancellationToken cancellationToken = default);
    Task<GenResult<AmazonUserProfile>> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<GenResult<AmazonUserProfile>> VerifyAndGetProfileAsync(string accessToken, string? expectedUserId = null, CancellationToken cancellationToken = default);
}
