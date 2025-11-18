using ID.OAuth.Amazon.Data;
using ID.OAuth.Amazon.HttpService.Abs;
using ID.OAuth.Amazon.Setup;
using ID.OAuth.Utils.Abs;
using LoggingHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyResults;
using System.Text.Json;

namespace ID.OAuth.Amazon.HttpService.Imps;
internal class AmazonHttpClient : IAmazonHttpClient
{
    private readonly HttpClient _client;
    private readonly IOAuthHttpClientUtils _oAuthUtils;
    private readonly IdOAuthAmazonOptions _opts;
    private readonly ILogger<AmazonHttpClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AmazonHttpClient(
        HttpClient client,
        IOAuthHttpClientUtils oAuthUtils,
        IOptions<IdOAuthAmazonOptions> optsProvider,
        ILogger<AmazonHttpClient> logger,
        JsonSerializerOptions jsonOptions)
    {
        _client = client;
        _oAuthUtils = oAuthUtils;
        _opts = optsProvider.Value;
        _logger = logger;
        _jsonOptions = jsonOptions;
    }

    public async Task<GenResult<AmazonTokenInfo>> GetTokenInfoAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return GenResult<AmazonTokenInfo>.BadRequestResult("Missing access token.");

        var relative = $"auth/o2/tokeninfo?access_token={Uri.EscapeDataString(accessToken)}";

        var response = await _client.GetAsync(relative, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return _oAuthUtils.MapResponseToResult<AmazonTokenInfo>(response, "Amazon", relative, body);

        AmazonTokenInfo? tokenInfo = null;
        try
        {
            tokenInfo = JsonSerializer.Deserialize<AmazonTokenInfo>(body, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize tokeninfo response. Endpoint: {Endpoint}. Body: {Body}", relative, body);
            return GenResult<AmazonTokenInfo>.Failure($"Failed to parse tokeninfo response. Body: {body}");
        }

        if (tokenInfo == null)
            return GenResult<AmazonTokenInfo>.Failure("Empty tokeninfo response.");

        if (tokenInfo.ExpiresIn.HasValue)
            tokenInfo.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenInfo.ExpiresIn.Value);

        return GenResult<AmazonTokenInfo>.Success(tokenInfo);
    }

    public async Task<GenResult<AmazonUserProfile>> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return GenResult<AmazonUserProfile>.BadRequestResult("Missing access token.");

        var req = new HttpRequestMessage(HttpMethod.Get, "user/profile");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _client.SendAsync(req, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return _oAuthUtils.MapResponseToResult<AmazonUserProfile>(response, "Amazon", "user/profile", body);

        AmazonUserProfile? profile = null;
        try
        {
            profile = JsonSerializer.Deserialize<AmazonUserProfile>(body, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize user profile. Endpoint: {Endpoint}. Body: {Body}", "user/profile", body);
            return GenResult<AmazonUserProfile>.Failure($"Failed to parse user profile response. Body: {body}");
        }

        if (profile == null)
            return GenResult<AmazonUserProfile>.Failure("Empty profile response.");

        return GenResult<AmazonUserProfile>.Success(profile);
    }
}
