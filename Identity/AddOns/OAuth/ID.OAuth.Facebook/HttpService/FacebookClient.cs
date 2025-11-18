using ID.GlobalSettings.Errors;
using ID.OAuth.Facebook.Data;
using ID.OAuth.Facebook.Services;
using ID.OAuth.Facebook.Setup;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyResults;
using System.Diagnostics;
using System.Text.Json;

namespace ID.OAuth.Facebook.HttpService;
internal class FacebookHttpClient(
    HttpClient _client,
    IFacebookClientUtilities _utilities,
    IOptions<IdOAuthFacebookOptions> _optsProvider,
    ILogger<FacebookHttpClient> _logger,
    JsonSerializerOptions _jsonOptions)
    : IFacebookHttpClient
{

    private readonly IdOAuthFacebookOptions _opts = _optsProvider.Value;

    //----------------------//

    public async Task<GenResult<FacebookDebugTokenData>> GetDebugTokenAsync(string userAccessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Get secrets
            var appId = _opts.AppId;
            var appSecret = _opts.AppSecret;


            // 2. Generate required values
            var appAccessToken = $"{appId}|{appSecret}";
            var appSecretProof = _utilities.GenerateAppSecretProof(userAccessToken);

            // 3. Build the Debug URL using UriBuilder

            // Build query safely
            var qs = new List<string>
        {
            $"input_token={Uri.EscapeDataString(userAccessToken ?? string.Empty)}",
            $"access_token={Uri.EscapeDataString(appAccessToken ?? string.Empty)}",
            $"appsecret_proof={Uri.EscapeDataString(appSecretProof ?? string.Empty)}"
        };

            var relative = "debug_token?" + string.Join("&", qs);


            // 4. Make the call
            var response = await _client.GetAsync(relative, cancellationToken);

            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return GenResult<FacebookDebugTokenData>.Failure($"Failed to retrieve debug token.:{jsonResponse}");

            // Deserialize the response into a C# class (you'll define this struct)

            var debugDataResponse = JsonSerializer.Deserialize<FacebookDebugTokenResponse>(jsonResponse, _jsonOptions);
            if (debugDataResponse?.Data == null)
                return GenResult<FacebookDebugTokenData>.Failure("Failed to parse debug token response.");

            return GenResult<FacebookDebugTokenData>.Success(debugDataResponse.Data);

        }
        catch(Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.OAuth.Facebook);
            return GenResult<FacebookDebugTokenData>.Failure(ex);
        }

    }

    //----------------------//

    /// <summary>
    /// Get the verified user profile from Facebook Graph API using the supplied user access token.
    /// </summary>
    /// <param name="userAccessToken">User access token obtained by the client</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>FacebookUserProfile on success; null if request failed or permission denied</returns>
    public async Task<GenResult<FacebookUserProfile>> GetUserProfileAsync(string userAccessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userAccessToken))
                return GenResult<FacebookUserProfile>.Failure("Missing user access token.");

            // Use appsecret_proof for additional security (utilities generates it)
            var appSecretProof = _utilities.GenerateAppSecretProof(userAccessToken);

            // Fields to request from Graph API (keep minimal, request additional scopes as needed)
            var fields = "id,name,email,first_name,last_name,picture.width(200).height(200){is_silhouette,url},gender,locale,birthday,timezone,verified";

            var qs = new List<string>
            {
                $"fields={Uri.EscapeDataString(fields)}",
                $"access_token={Uri.EscapeDataString(userAccessToken)}",
                $"appsecret_proof={Uri.EscapeDataString(appSecretProof ?? string.Empty)}"
            };

            var relative = "me?" + string.Join("&", qs);

            // 4. Make the call
            var response = await _client.GetAsync(relative, cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return GenResult<FacebookUserProfile>.Failure($"Failed to retrieve user profile.:{json}");

            var profile = JsonSerializer.Deserialize<FacebookUserProfile>(json, _jsonOptions);
            if (profile == null)
                return GenResult<FacebookUserProfile>.Failure("Failed to parse user profile response.");

            return GenResult<FacebookUserProfile>.Success(profile);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.OAuth.Facebook);
            return GenResult<FacebookUserProfile>.Failure(ex);
        }
    }



}//Cls
