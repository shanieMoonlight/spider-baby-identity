//using ID.GlobalSettings.Errors;
//using ID.OAuth.Facebook.Auth.Abs;
//using ID.OAuth.Facebook.Data;
//using ID.OAuth.Facebook.Setup;
//using LoggingHelpers;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using MyResults;
//using System.Text.Json;

//namespace ID.OAuth.Facebook.Auth.Imps;

///// <summary>
///// Facebook token verifier that validates access tokens server-side using Facebook's Graph API.
///// Implements secure two-step verification: token validation + user profile retrieval.
///// </summary>
//internal class FacebookTokenVerifier(
//    IOptions<IdOAuthFacebookOptions> optionsProvider,
//    IHttpClientFactory httpClientFactory,
//    ILogger<FacebookTokenVerifier> logger) : IFacebookTokenVerifier
//{
//    private readonly IdOAuthFacebookOptions _options = optionsProvider.Value;
//    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
//    private readonly ILogger<FacebookTokenVerifier> _logger = logger;

//    public async Task<GenResult<FacebookVerifiedPayload>> VerifyTokenAsync(string accessToken, CancellationToken cancellationToken)
//    {
//        try
//        {
//            // Input validation
//            if (string.IsNullOrWhiteSpace(accessToken))
//            {
//                return GenResult<FacebookVerifiedPayload>.UnauthorizedResult("Access token is required");
//            }

//            // Step 1: Verify token with Facebook's debug endpoint
//            var tokenValidation = await ValidateTokenWithFacebookAsync(accessToken, cancellationToken);
//            if (!tokenValidation.IsValid)
//            {
//                _logger.LogWarning("Facebook token validation failed for token: {TokenPrefix}...", 
//                    accessToken.Length > 10 ? accessToken[..10] : accessToken);
//                return GenResult<FacebookVerifiedPayload>.UnauthorizedResult("Invalid Facebook access token");
//            }

//            // Step 2: Get user profile data using the verified token
//            var userProfile = await GetUserProfileAsync(accessToken, cancellationToken);
//            if (userProfile == null)
//            {
//                _logger.LogWarning("Failed to retrieve Facebook user profile for validated token");
//                return GenResult<FacebookVerifiedPayload>.UnauthorizedResult("Failed to retrieve user profile");
//            }

//            _logger.LogInformation("Successfully verified Facebook token for user: {UserId}", userProfile.Id);
//            return GenResult<FacebookVerifiedPayload>.Success(userProfile);
//        }
//        catch (HttpRequestException ex)
//        {
//            _logger.LogException(ex, IdErrorEvents.OAuth.Verification);
//            return GenResult<FacebookVerifiedPayload>.UnauthorizedResult("Facebook API request failed");
//        }
//        catch (JsonException ex)
//        {
//            _logger.LogException(ex, IdErrorEvents.OAuth.Verification);
//            return GenResult<FacebookVerifiedPayload>.UnauthorizedResult("Invalid response from Facebook API");
//        }
//        catch (TaskCanceledException ex)
//        {
//            _logger.LogException(ex, IdErrorEvents.OAuth.Verification);
//            return GenResult<FacebookVerifiedPayload>.UnauthorizedResult("Facebook API request timed out");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogException(ex, IdErrorEvents.OAuth.Verification);
//            return GenResult<FacebookVerifiedPayload>.UnauthorizedResult(ex.Message);
//        }
//    }

//    //----------------------//    
    
//    /// <summary>
//    /// Validates the access token with Facebook's debug_token endpoint.
//    /// Ensures the token is valid, belongs to our app, and hasn't expired.
//    /// </summary>
//    private async Task<(bool IsValid, string? UserId)> ValidateTokenWithFacebookAsync(string accessToken, CancellationToken cancellationToken)
//    {
//        var client = _httpClientFactory.CreateClient(FacebookHttpClientConfiguration.FacebookOAuthClientName);
        
//        // Facebook's token debug endpoint - verifies token authenticity
//        var debugUrl = $"/{_options.GraphApiVersion}/debug_token" +
//                      $"?input_token={Uri.EscapeDataString(accessToken)}" +
//                      $"&access_token={Uri.EscapeDataString(_options.AppId)}|{Uri.EscapeDataString(_options.AppSecret)}";

//        var response = await client.GetAsync(debugUrl, cancellationToken);
        
//        if (!response.IsSuccessStatusCode)
//        {
//            _logger.LogWarning("Facebook debug_token endpoint returned {StatusCode}: {ReasonPhrase}", 
//                response.StatusCode, response.ReasonPhrase);
//            return (false, null);
//        }

//        var content = await response.Content.ReadAsStringAsync(cancellationToken);
//        var debugResult = JsonSerializer.Deserialize<FacebookDebugResponse>(content);

//        if (debugResult?.Data == null)
//        {
//            _logger.LogWarning("Invalid response structure from Facebook debug_token endpoint");
//            return (false, null);
//        }

//        var tokenData = debugResult.Data;
        
//        // Verify token belongs to our app and is valid
//        var isValid = tokenData.IsValid && 
//                     tokenData.AppId == _options.AppId &&
//                     (!tokenData.ExpiresAt.HasValue || 
//                      DateTimeOffset.FromUnixTimeSeconds(tokenData.ExpiresAt.Value) > DateTimeOffset.UtcNow);

//        if (!isValid)
//        {
//            _logger.LogWarning("Token validation failed - IsValid: {IsValid}, AppId: {AppId}, Expected: {ExpectedAppId}, ExpiresAt: {ExpiresAt}",
//                tokenData.IsValid, tokenData.AppId, _options.AppId, tokenData.ExpiresAt);
//        }

//        return (isValid, tokenData.UserId);
//    }

//    //----------------------//    
    
//    /// <summary>
//    /// Retrieves verified user profile data from Facebook's Graph API.
//    /// Only called after token validation succeeds.
//    /// </summary>
//    private async Task<FacebookVerifiedPayload?> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken)
//    {
//        var client = _httpClientFactory.CreateClient(FacebookHttpClientConfiguration.FacebookOAuthClientName);
        
//        // Request user profile with specific fields - only get what we need
//        var fields = "id,email,name,first_name,last_name,picture.type(large),gender,locale,timezone,birthday";
//        var profileUrl = $"/{_options.GraphApiVersion}/me" +
//                        $"?fields={Uri.EscapeDataString(fields)}" +
//                        $"&access_token={Uri.EscapeDataString(accessToken)}";

//        var response = await client.GetAsync(profileUrl, cancellationToken);
        
//        if (!response.IsSuccessStatusCode)
//        {
//            _logger.LogWarning("Facebook Graph API /me endpoint returned {StatusCode}: {ReasonPhrase}", 
//                response.StatusCode, response.ReasonPhrase);
//            return null;
//        }

//        var content = await response.Content.ReadAsStringAsync(cancellationToken);
//        var profile = JsonSerializer.Deserialize<FacebookUserProfile>(content);
        
//        if (profile == null)
//        {
//            _logger.LogWarning("Failed to deserialize Facebook user profile response");
//            return null;
//        }

//        // Convert to our verified payload model
//        return new FacebookVerifiedPayload
//        {
//            Id = profile.Id ?? string.Empty,
//            Email = profile.Email ?? string.Empty,
//            Name = profile.Name ?? string.Empty,
//            FirstName = profile.FirstName ?? string.Empty,
//            LastName = profile.LastName ?? string.Empty,
//            Picture = profile.Picture?.Data?.Url ?? string.Empty,
//            EmailVerified = !string.IsNullOrEmpty(profile.Email), // Facebook emails are generally verified
//            Gender = profile.Gender,
//            Locale = profile.Locale,
//            Timezone = profile.Timezone,
//            Birthday = ParseFacebookBirthday(profile.Birthday)
//        };
//    }    //----------------------//

//    /// <summary>
//    /// Parses Facebook birthday format (MM/dd/yyyy or MM/dd) to DateTime.
//    /// </summary>
//    private static DateTime? ParseFacebookBirthday(string? birthday)
//    {
//        if (string.IsNullOrWhiteSpace(birthday))
//            return null;

//        // Facebook returns birthday in MM/dd/yyyy or MM/dd format
//        if (DateTime.TryParseExact(birthday, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out var fullDate))
//            return fullDate;
        
//        if (DateTime.TryParseExact(birthday, "MM/dd", null, System.Globalization.DateTimeStyles.None, out var monthDay))
//            return new DateTime(DateTime.MinValue.Year, monthDay.Month, monthDay.Day);

//        return null;
//    }

//}//Cls
