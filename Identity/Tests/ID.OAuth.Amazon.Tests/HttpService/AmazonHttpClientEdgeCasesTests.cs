using ID.OAuth.Utils.Services.Abs;

namespace ID.OAuth.Amazon.Tests.HttpService;

public class AmazonHttpClientEdgeCasesTests
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

    private class TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => 
            Task.FromResult(responder(request));
    }

    //--------------------------//

    [Fact]
    public async Task GetTokenInfoAsync_DeserializationFailure_LogsWarningAndReturnsFailure()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{notjson}") });
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazon.com/") };

        var mockOAuthUtils = new Mock<IOAuthHttpClientUtils>();
        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "app" });
        var mockLogger = new Mock<ILogger<AmazonHttpClient>>();
        var amazon = new AmazonHttpClient(client, mockOAuthUtils.Object, opts, mockLogger.Object, CreateJsonOptions());

        // Act
        var result = await amazon.GetTokenInfoAsync("token");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.Failure);
        result.Info.ShouldContain("Failed to parse tokeninfo response", Case.Insensitive);

        mockLogger.VerifyWarningLogging<AmazonHttpClient>(msg => msg.ToString()?.Contains("Failed to deserialize tokeninfo response") == true);
    }

    //--------------------------//

    [Fact]
    public async Task GetUserProfileAsync_DeserializationFailure_LogsWarningAndReturnsFailure()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{notjson}") });
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazon.com/") };

        var mockOAuthUtils = new Mock<IOAuthHttpClientUtils>();
        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "app" });
        var mockLogger = new Mock<ILogger<AmazonHttpClient>>();
        var amazon = new AmazonHttpClient(client, mockOAuthUtils.Object, opts, mockLogger.Object, CreateJsonOptions());

        // Act
        var result = await amazon.GetUserProfileAsync("token");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.Failure);
        result.Info.ShouldContain("Failed to parse user profile response", Case.Insensitive);

        mockLogger.VerifyWarningLogging<AmazonHttpClient>(msg => msg.ToString()?.Contains("Failed to deserialize user profile") == true);
    }

    //--------------------------//

    [Fact]
    public async Task GetTokenInfoAsync_MapsUnauthorized_WhenHttpClientReturnsUnauthorized()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("unauth") });
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazon.com/") };

        var mockOAuthUtils = new Mock<IOAuthHttpClientUtils>();
        mockOAuthUtils.Setup(u => u.MapResponseToResult<AmazonTokenInfo>(It.IsAny<HttpResponseMessage>(), "Amazon", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(GenResult<AmazonTokenInfo>.UnauthorizedResult("unauthorized"));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "app" });
        var amazon = new AmazonHttpClient(client, mockOAuthUtils.Object, opts, Mock.Of<ILogger<AmazonHttpClient>>(), CreateJsonOptions());

        // Act
        var result = await amazon.GetTokenInfoAsync("token");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.Unauthorized);
    }

    //--------------------------//

    [Fact]
    public async Task GetUserProfileAsync_MapsForbidden_WhenHttpClientReturnsForbidden()
    {
        // Arrange
        var handler = new TestHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent("forbidden") });
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazon.com/") };

        var mockOAuthUtils = new Mock<IOAuthHttpClientUtils>();
        mockOAuthUtils.Setup(u => u.MapResponseToResult<AmazonUserProfile>(It.IsAny<HttpResponseMessage>(), "Amazon", It.IsAny<string>(), It.IsAny<string>()))
            .Returns(GenResult<AmazonUserProfile>.ForbiddenResult("forbidden"));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "app" });
        var amazon = new AmazonHttpClient(client, mockOAuthUtils.Object, opts, Mock.Of<ILogger<AmazonHttpClient>>(), CreateJsonOptions());

        // Act
        var result = await amazon.GetUserProfileAsync("token");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.Forbidden);
    }
}
