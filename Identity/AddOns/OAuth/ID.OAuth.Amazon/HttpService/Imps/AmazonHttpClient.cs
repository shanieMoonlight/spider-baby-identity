using ID.OAuth.Amazon.Data;
using ID.OAuth.Amazon.HttpService.Abs;
using ID.OAuth.Amazon.Setup;
using ID.OAuth.Utils.Abs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyResults;
using System.Text.Json;

namespace ID.OAuth.Amazon.HttpService.Imps;
internal class AmazonHttpClient(
    HttpClient client,
    IOAuthHttpClientUtils oAuthUtils,
    IOptions<IdOAuthAmazonOptions> optsProvider,
    ILogger<AmazonHttpClient> logger,
    JsonSerializerOptions jsonOptions) : IAmazonHttpClient
{
    private readonly IdOAuthAmazonOptions _opts = optsProvider.Value;

    //--------------------------//

    public async Task<GenResult<AmazonTokenInfo>> GetTokenInfoAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return GenResult<AmazonTokenInfo>.BadRequestResult("Missing access token.");

        var relative = $"auth/o2/{AmazonApi.Endpoints.TokenInfo}?access_token={Uri.EscapeDataString(accessToken)}";

        var response = await client.GetAsync(relative, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return oAuthUtils.MapResponseToResult<AmazonTokenInfo>(response, "Amazon", relative, body);

        AmazonTokenInfo? tokenInfo;
        try
        {
            tokenInfo = JsonSerializer.Deserialize<AmazonTokenInfo>(body, jsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to deserialize tokeninfo response. Endpoint: {Endpoint}. Body: {Body}", relative, body);
            return GenResult<AmazonTokenInfo>.Failure($"Failed to parse tokeninfo response. Body: {body}");
        }

        if (tokenInfo == null)
            return GenResult<AmazonTokenInfo>.Failure("Empty tokeninfo response.");

        return GenResult<AmazonTokenInfo>.Success(tokenInfo);
    }

    //--------------------------//

    public async Task<GenResult<AmazonUserProfile>> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return GenResult<AmazonUserProfile>.BadRequestResult("Missing access token.");

        var userProfilePath = AmazonApi.Endpoints.UserProfile;
        var req = new HttpRequestMessage(HttpMethod.Get, userProfilePath);
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.SendAsync(req, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return oAuthUtils.MapResponseToResult<AmazonUserProfile>(response, "Amazon", userProfilePath, body);

        AmazonUserProfile? profile;
        try
        {
            profile = JsonSerializer.Deserialize<AmazonUserProfile>(body, jsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to deserialize user profile. Endpoint: {Endpoint}. Body: {Body}", userProfilePath, body);
            return GenResult<AmazonUserProfile>.Failure($"Failed to parse user profile response. Body: {body}");
        }

        if (profile == null)
            return GenResult<AmazonUserProfile>.Failure("Empty profile response.");

        return GenResult<AmazonUserProfile>.Success(profile);
    }

}//Cls
