using ID.GlobalSettings.Errors;
using ID.OAuth.Amazon.Data;
using ID.OAuth.Amazon.HttpService.Abs;
using ID.OAuth.Amazon.Services.Abs;
using ID.OAuth.Amazon.Setup;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyResults;
using StringHelpers;

namespace ID.OAuth.Amazon.Services.Imps;

internal sealed partial class AmazonAuthenticationService(
    IAmazonHttpClient _http,
    IOptions<IdOAuthAmazonOptions> _optsProvider,
    ILogger<AmazonAuthenticationService> _logger)
    : IAmazonAuthenticationService
{
    private readonly IdOAuthAmazonOptions _opts = _optsProvider.Value;

    public async Task<GenResult<AmazonTokenInfo>> VerifyTokenAsync(string accessToken, string expectedUserId, CancellationToken cancellationToken = default)
    {
        if (accessToken.IsNullOrWhiteSpace())
            return GenResult<AmazonTokenInfo>.BadRequestResult("empty_access_token");

        if (_opts.ClientId.IsNullOrWhiteSpace())
            return GenResult<AmazonTokenInfo>.Failure("missing_server_credentials: ClientId");

        try
        {
            var tokenInfoResult = await _http.GetTokenInfoAsync(accessToken, cancellationToken);
            if (!tokenInfoResult.Succeeded)
                return tokenInfoResult;

            var tokenInfo = tokenInfoResult.Value!;

            if (tokenInfo.ClientId != _opts.ClientId)
                return GenResult<AmazonTokenInfo>.UnauthorizedResult("Token was not issued for this application.");

            if (!string.IsNullOrWhiteSpace(expectedUserId) && tokenInfo.UserId != expectedUserId)
                return GenResult<AmazonTokenInfo>.UnauthorizedResult("Token user ID does not match expected ID.");

            if (tokenInfo.ExpiresAt.HasValue && tokenInfo.ExpiresAt.Value <= DateTimeOffset.UtcNow)
                return GenResult<AmazonTokenInfo>.UnauthorizedResult("Token has expired.");

            return GenResult<AmazonTokenInfo>.Success(tokenInfo);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.OAuth.Amazon);
            return GenResult<AmazonTokenInfo>.Failure(ex);
        }
    }

    public async Task<GenResult<AmazonUserProfile>> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (accessToken.IsNullOrWhiteSpace())
            return GenResult<AmazonUserProfile>.BadRequestResult("empty_access_token");

        try
        {
            var profileResult = await _http.GetUserProfileAsync(accessToken, cancellationToken);
            if (!profileResult.Succeeded)
                return profileResult;

            var profile = profileResult.Value!;
            if (string.IsNullOrWhiteSpace(profile?.UserId))
                return GenResult<AmazonUserProfile>.Failure("Failed to retrieve user profile or missing user id.");

            return GenResult<AmazonUserProfile>.Success(profile);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.OAuth.Amazon);
            return GenResult<AmazonUserProfile>.Failure(ex);
        }
    }

    public async Task<GenResult<AmazonUserProfile>> VerifyAndGetProfileAsync(string accessToken, string? expectedUserId = null, CancellationToken cancellationToken = default)
    {
        var verifyResult = await VerifyTokenAsync(accessToken, expectedUserId ?? string.Empty, cancellationToken);
        if (!verifyResult.Succeeded)
            return verifyResult.Convert<AmazonUserProfile>();

        var tokenInfo = verifyResult.Value!;

        var profileResult = await GetUserProfileAsync(accessToken, cancellationToken);
        if (!profileResult.Succeeded)
            return profileResult;

        var profile = profileResult.Value!;

        if (!string.Equals(profile.UserId, tokenInfo.UserId, StringComparison.Ordinal))
            return GenResult<AmazonUserProfile>.UnauthorizedResult("Profile id does not match token user id.");

        return GenResult<AmazonUserProfile>.Success(profile);
    }
}
