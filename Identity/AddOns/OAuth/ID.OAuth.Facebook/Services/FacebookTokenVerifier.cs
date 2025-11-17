using ID.GlobalSettings.Errors;
using ID.OAuth.Facebook.Data;
using ID.OAuth.Facebook.HttpService;
using ID.OAuth.Facebook.Setup;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyResults;
using StringHelpers;

namespace ID.OAuth.Facebook.Services;

internal sealed partial class FacebookAuthenticationService(
    IFacebookHttpClient _http,
    IOptions<IdOAuthFacebookOptions> optsProvider,
    ILogger<FacebookAuthenticationService> _logger)
    : IFacebookAuthenticationService
{
    private readonly IdOAuthFacebookOptions _opts = optsProvider.Value;

    //----------------------//

    public async Task<GenResult<FacebookDebugTokenData>> VerifyTokenAsync(string authToken, string expectedUserId, CancellationToken cancellationToken = default)
    {
        if (authToken.IsNullOrWhiteSpace())
            return GenResult<FacebookDebugTokenData>.Failure("empty_user_token");

        if (_opts.AppId.IsNullOrWhiteSpace())
            return GenResult<FacebookDebugTokenData>.Failure("missing_server_credentials: AppId");

        if (_opts.AppSecret.IsNullOrWhiteSpace())
            return GenResult<FacebookDebugTokenData>.Failure("missing_server_credentials: AppSecret");

        try
        {
            var appToken = $"{_opts.AppId}|{_opts.AppSecret}";
            var url = $"debug_token?input_token={Uri.EscapeDataString(authToken)}&access_token={Uri.EscapeDataString(appToken)}";

            var debugDataResult = await _http.GetDebugTokenAsync(authToken, cancellationToken);

            if(!debugDataResult.Succeeded)
                return debugDataResult;

            var debugData = debugDataResult.Value!; //Success is non-null

            // 5. Perform the Critical Security Checks (just like in server.js)
            if (!debugData.IsValid)
                return GenResult<FacebookDebugTokenData>.Failure($"Token is invalid or expired. DebugData: {debugData}");

            if (debugData.AppId != _opts.AppId)
                return GenResult<FacebookDebugTokenData>.Failure($"Token was not issued for this application. DebugData: {debugData}");

            if (debugData.UserId != expectedUserId)
                return GenResult<FacebookDebugTokenData>.Failure($"Token user ID does not match expected ID. UserId: {expectedUserId}. DebugData: {debugData}");
            
            var expiresAt = debugData.ExpiresAt.HasValue ? DateTimeOffset.FromUnixTimeSeconds(debugData.ExpiresAt.Value) : (DateTimeOffset?)null;
            if (expiresAt.HasValue && expiresAt.Value <= DateTimeOffset.UtcNow)
                return GenResult<FacebookDebugTokenData>.Failure($"Token has expired. DebugData: {debugData}");

            return GenResult<FacebookDebugTokenData>.Success(debugData);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.OAuth.Facebook);
            return GenResult<FacebookDebugTokenData>.Failure(ex);
        }
    }

    //----------------------//

    public async Task<GenResult<FacebookUserProfile>> GetUserProfileAsync(string userAccessToken, CancellationToken cancellationToken = default)
    {
        if (userAccessToken.IsNullOrWhiteSpace())
            return GenResult<FacebookUserProfile>.Failure("empty_user_token");

        if (_opts.AppId.IsNullOrWhiteSpace())
            return GenResult<FacebookUserProfile>.Failure("missing_server_credentials: AppId");

        if (_opts.AppSecret.IsNullOrWhiteSpace())
            return GenResult<FacebookUserProfile>.Failure("missing_server_credentials: AppSecret");

        try
        {
            var profileResult = await _http.GetUserProfileAsync(userAccessToken, cancellationToken);

            if (!profileResult.Succeeded)
                return profileResult;

            var profile = profileResult.Value!; // Success guarantees non-null

            if (string.IsNullOrWhiteSpace(profile?.Id))
                return GenResult<FacebookUserProfile>.Failure("Failed to retrieve user profile or missing user id.");

            return GenResult<FacebookUserProfile>.Success(profile);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.OAuth.Facebook);
            return GenResult<FacebookUserProfile>.Failure(ex);
        }
    }

    //----------------------//

    /// <summary>
    /// Verify the supplied user access token using debug_token and then fetch the verified profile (/me).
    /// Returns a GenResult containing the verified user profile on success.
    /// </summary>
    public async Task<GenResult<FacebookUserProfile>> VerifyAndGetProfileAsync(
        string userAccessToken,
        string? expectedUserId = null,
        CancellationToken cancellationToken = default)
    {
        // 1. Verify token
        var verifyResult = await VerifyTokenAsync(userAccessToken, expectedUserId ?? string.Empty, cancellationToken);
        if (!verifyResult.Succeeded)
        {
            // propagate failure info
            return GenResult<FacebookUserProfile>.Failure(verifyResult.Info ?? "Token verification failed.");
        }

        var debug = verifyResult.Value!;

        // Additional security checks already done in VerifyTokenAsync (IsValid, AppId, UserId, Expiry)

        // 2. Fetch profile
        var profileResult = await GetUserProfileAsync(userAccessToken, cancellationToken);
        if (!profileResult.Succeeded)
        {
            return GenResult<FacebookUserProfile>.Failure(profileResult.Info ?? "Failed to fetch profile.");
        }

        var profile = profileResult.Value!;

        // 3. Ensure ids match
        if (!string.Equals(profile.Id, debug.UserId, StringComparison.Ordinal))
        {
            return GenResult<FacebookUserProfile>.Failure("Profile id does not match token user id.");
        }

        return GenResult<FacebookUserProfile>.Success(profile);
    }

}//Cls
