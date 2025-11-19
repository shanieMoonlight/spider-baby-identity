using ID.OAuth.Facebook.HttpService.Imps;
using ID.OAuth.Facebook.Tests.HttpService;
using ID.OAuth.Utils.Abs;

namespace ID.OAuth.Facebook.Tests;

public class FacebookClientRateLimitTests
{
    private static JsonSerializerOptions CreateJsonOptions() => new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    [Fact]
    public async Task GetDebugTokenAsync_ReturnsRateLimitResult_When429()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.TooManyRequests) { Content = new StringContent("too many") });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var fbUtilities = new FacebookClientUtilities(opts);
        var mockGenUtils = new Mock<IOAuthHttpClientUtils>();

        // Map any non-success response to RateLimitExceeded
        mockGenUtils.Setup(u => u.MapResponseToResult<ID.OAuth.Facebook.Data.FacebookDebugTokenData>(It.IsAny<HttpResponseMessage>(), "Facebook", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(MyResults.GenResult<ID.OAuth.Facebook.Data.FacebookDebugTokenData>.RateLimitExceededResult("rate_limited: too many"));

        var logger = Mock.Of<ILogger<FacebookHttpClient>>();
        var jsonOpts = CreateJsonOptions();

        var fb = new FacebookHttpClient(client, fbUtilities, mockGenUtils.Object, opts, logger, jsonOpts);

        // Act
        var result = await fb.GetDebugTokenAsync("token");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(MyResults.BasicResult.ResultStatus.RateLimitExceeded);
        result.Info.ShouldContain("rate_limited", Case.Insensitive);
    }

    [Fact]
    public async Task GetDebugTokenAsync_DoesNotRetry_On429()
    {
        // Arrange
        var calls = 0;
        var handler = new TestHttpMessageHandler(req =>
        {
            calls++;
            if (calls == 1)
                return new HttpResponseMessage(HttpStatusCode.TooManyRequests) { Content = new StringContent("too many") };
            // would succeed on second call if retried
            var debugJson = "{\"data\":{\"app_id\":\"app123\",\"is_valid\":true,\"user_id\":\"user_1\",\"expires_at\":9999999999}}";
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(debugJson) };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var utilities = new FacebookClientUtilities(opts);
        var logger = Mock.Of<ILogger<FacebookHttpClient>>();
        var jsonOpts = CreateJsonOptions();

        var mockGenUtils = new Mock<IOAuthHttpClientUtils>();
        mockGenUtils.Setup(u => u.MapResponseToResult<ID.OAuth.Facebook.Data.FacebookDebugTokenData>(It.IsAny<HttpResponseMessage>(), "Facebook", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(MyResults.GenResult<ID.OAuth.Facebook.Data.FacebookDebugTokenData>.RateLimitExceededResult("rate_limited: too many"));

        var fb = new FacebookHttpClient(client, utilities, mockGenUtils.Object, opts, logger, jsonOpts);

        // Act
        var result = await fb.GetDebugTokenAsync("token");

        // Assert - ensure only one call was made (no retry) and rate-limit result returned
        calls.ShouldBe(1);
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(MyResults.BasicResult.ResultStatus.RateLimitExceeded);
    }

    [Fact]
    public async Task GetUserProfileAsync_ReturnsRateLimitResult_When429()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.TooManyRequests) { Content = new StringContent("too many") });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var utilities = new FacebookClientUtilities(opts);
        var logger = Mock.Of<ILogger<FacebookHttpClient>>();
        var jsonOpts = CreateJsonOptions();

        var mockGenUtils = new Mock<IOAuthHttpClientUtils>();
        mockGenUtils.Setup(u => u.MapResponseToResult<ID.OAuth.Facebook.Data.FacebookUserProfile>(It.IsAny<HttpResponseMessage>(), "Facebook", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(MyResults.GenResult<ID.OAuth.Facebook.Data.FacebookUserProfile>.RateLimitExceededResult("rate_limited: too many"));

        var fb = new FacebookHttpClient(client, utilities, mockGenUtils.Object, opts, logger, jsonOpts);

        // Act
        var result = await fb.GetUserProfileAsync("token");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(MyResults.BasicResult.ResultStatus.RateLimitExceeded);
        result.Info.ShouldContain("rate_limited", Case.Insensitive);
    }
}
