using ID.OAuth.Utils.Services.Abs;

namespace ID.OAuth.Amazon.Tests.HttpService;

public class AmazonHttpClientTests
{
    private static JsonSerializerOptions CreateJsonOptions()
    {
        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
        opts.Converters.Add(new UnixEpochSecondsJsonConverter());
        return opts;
    }

    //--------------------------//

    private class TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(responder(request));
    }

    //--------------------------//

    [Fact]
    public async Task GetTokenInfoAsync_ReturnsSuccess_When200()
    {
        // Arrange
        var tokenJson = "{\"aud\":\"amzn1.application-oa2-client.appid\",\"user_id\":\"amzn1.account.UID\",\"iss\":\"https://www.amazon.com\",\"exp\":600,\"app_id\":\"amzn1.application.appid\",\"iat\":1633036800}";

        var handler = new TestHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(tokenJson) });
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazon.com/") };

        var mockOAuthUtils = new Mock<IOAuthHttpClientUtils>();
        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "amzn1.application-oa2-client.appid" });
        var logger = Mock.Of<ILogger<AmazonHttpClient>>();
        var jsonOpts = CreateJsonOptions();

        var amazon = new AmazonHttpClient(client, mockOAuthUtils.Object, opts, logger, jsonOpts);

        // Act
        var result = await amazon.GetTokenInfoAsync("token123");

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ClientId.ShouldBe("amzn1.application-oa2-client.appid");
        result.Value.UserId.ShouldBe("amzn1.account.UID");
        result.Value.ExpiresIn.ShouldBe(600);
        result.Value.ExpiresAt.HasValue.ShouldBeTrue();
    }

    //--------------------------//

    [Fact]
    public async Task GetTokenInfoAsync_ReturnsRateLimit_When429()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(req => new HttpResponseMessage((HttpStatusCode)429) { Content = new StringContent("too many") });
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazon.com/") };

        var mockOAuthUtils = new Mock<IOAuthHttpClientUtils>();
        mockOAuthUtils.Setup(u => u.MapResponseToResult<AmazonTokenInfo>(It.IsAny<HttpResponseMessage>(), "Amazon", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(GenResult<AmazonTokenInfo>.RateLimitExceededResult("rate_limited: too many"));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "app" });
        var amazon = new AmazonHttpClient(client, mockOAuthUtils.Object, opts, Mock.Of<ILogger<AmazonHttpClient>>(), CreateJsonOptions());

        // Act
        var result = await amazon.GetTokenInfoAsync("token123");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.RateLimitExceeded);
    }

    //--------------------------//

    [Fact]
    public async Task GetUserProfileAsync_ReturnsSuccess_When200()
    {
        // Arrange
        var profileJson = "{\"user_id\":\"amzn1.account.UID\",\"name\":\"Test Name\",\"email\":\"me@example.com\"}";
        var handler = new TestHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(profileJson) });
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazon.com/") };

        var mockOAuthUtils = new Mock<IOAuthHttpClientUtils>();
        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "app" });
        var amazon = new AmazonHttpClient(client, mockOAuthUtils.Object, opts, Mock.Of<ILogger<AmazonHttpClient>>(), CreateJsonOptions());

        // Act
        var result = await amazon.GetUserProfileAsync("token123");

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.UserId.ShouldBe("amzn1.account.UID");
        result.Value.Email.ShouldBe("me@example.com");
    }

    //--------------------------//

    [Fact]
    public async Task GetUserProfileAsync_ReturnsRateLimit_When429()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(req => new HttpResponseMessage((HttpStatusCode)429) { Content = new StringContent("too many") });
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazon.com/") };

        var mockOAuthUtils = new Mock<IOAuthHttpClientUtils>();
        mockOAuthUtils.Setup(u => u.MapResponseToResult<AmazonUserProfile>(It.IsAny<HttpResponseMessage>(), "Amazon", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(GenResult<AmazonUserProfile>.RateLimitExceededResult("rate_limited: too many"));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "app" });
        var amazon = new AmazonHttpClient(client, mockOAuthUtils.Object, opts, Mock.Of<ILogger<AmazonHttpClient>>(), CreateJsonOptions());

        // Act
        var result = await amazon.GetUserProfileAsync("token123");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.RateLimitExceeded);
    }

}//Cls
