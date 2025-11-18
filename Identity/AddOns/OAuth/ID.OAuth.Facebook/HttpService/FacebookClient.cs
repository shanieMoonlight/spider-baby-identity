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
                return MapResponseToResult<FacebookDebugTokenData>(response, relative, jsonResponse);

            // Deserialize the response into a C# class (you'll define this struct)

            FacebookDebugTokenResponse? debugDataResponse = null;
            try
            {
                debugDataResponse = JsonSerializer.Deserialize<FacebookDebugTokenResponse>(jsonResponse, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize debug_token response. Endpoint: {Endpoint}. Body: {Body}", relative, jsonResponse);
                return GenResult<FacebookDebugTokenData>.Failure($"Failed to parse debug token response. Body: {jsonResponse}");
            }

            if (debugDataResponse?.Data == null)
            {
                _logger.LogWarning("Parsed debug_token response did not contain expected 'data' property. Endpoint: {Endpoint}. Body: {Body}", relative, jsonResponse);
                return GenResult<FacebookDebugTokenData>.Failure($"Failed to parse debug token response. Body: {jsonResponse}");
            }

            return GenResult<FacebookDebugTokenData>.Success(debugDataResponse.Data);

        }
        catch(Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.OAuth.Facebook);
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.StackTrace);
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
                return GenResult<FacebookUserProfile>.BadRequestResult("Missing user access token.");

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
                return MapResponseToResult<FacebookUserProfile>(response, relative, json);

            FacebookUserProfile? profile = null;
            try
            {
                profile = JsonSerializer.Deserialize<FacebookUserProfile>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize /me response. Endpoint: {Endpoint}. Body: {Body}", relative, json);
                return GenResult<FacebookUserProfile>.Failure($"Failed to parse user profile response. Body: {json}");
            }

            if (profile == null)
            {
                _logger.LogWarning("Parsed /me response was null. Endpoint: {Endpoint}. Body: {Body}", relative, json);
                return GenResult<FacebookUserProfile>.Failure("Failed to parse user profile response.");
            }

            return GenResult<FacebookUserProfile>.Success(profile);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.OAuth.Facebook);
            return GenResult<FacebookUserProfile>.Failure(ex);
        }
    }

    //----------------------//


    // Helper to map non-success HTTP responses to GenResult<T>
    private GenResult<T> MapResponseToResult<T>(HttpResponseMessage response, string endpoint, string body)
    {
        // Log details to aid debugging
        _logger.LogWarning("Facebook request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}, Response: {Response}", response.StatusCode, endpoint, body);

        var info = $"StatusCode: {(int)response.StatusCode}. Body: {body}";

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return GenResult<T>.UnauthorizedResult(info);

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            return GenResult<T>.ForbiddenResult(info);

        if ((int)response.StatusCode == 429)
            return GenResult<T>.Failure($"rate_limited: {info}");

        if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            return GenResult<T>.BadRequestResult(info);

        return GenResult<T>.Failure($"Request failed. {info}");
    }


}//Cls
