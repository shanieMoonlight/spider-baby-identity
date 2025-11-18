using ID.Tests.Utility.Logging;
using System.Text.Json;

namespace ID.OAuth.Facebook.Tests;

//###########################################################///

internal class TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(responder(request));
    }
}

//###########################################################///

public class FacebookHttpClientTests
{
    private static JsonSerializerOptions CreateJsonOptionsWithConverter()
    {
        var jsonOpts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        jsonOpts.Converters.Add(new UnixEpochSecondsJsonConverter());
        return jsonOpts;
    }

    //----------------------//

    [Fact]
    public async Task GetDebugTokenAsync_ReturnsSuccess_WhenResponseIs200AndValidJson()
    {
        // Arrange
        var userToken = "some_user_token";
        var debugJson = "{\"data\":{\"app_id\":\"app123\",\"is_valid\":true,\"user_id\":\"user_1\",\"expires_at\":9999999999}}";

        var handler = new TestHttpMessageHandler(req =>
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(debugJson)
            };
            return resp;
        });

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://graph.facebook.com/v18.0/")
        };

        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var utilities = new FacebookClientUtilities(opts);
        var logger = Mock.Of<ILogger<FacebookHttpClient>>();

        var jsonOpts = CreateJsonOptionsWithConverter();

        var fb = new FacebookHttpClient(client, utilities, opts, logger, jsonOpts);

        // Act
        var result = await fb.GetDebugTokenAsync(userToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.AppId.ShouldBe("app123");
        result.Value.IsValid.ShouldBeTrue();
        result.Value.UserId.ShouldBe("user_1");
    }

    //----------------------//

    [Fact]
    public async Task GetUserProfileAsync_ReturnsSuccess_WhenResponseIs200AndValidJson()
    {
        // Arrange
        var userToken = "some_user_token";
        var profileJson = @"{""id"": ""101"", ""email"": ""me@example.com"", ""name"": ""Test Me"", ""first_name"": ""Test"", ""last_name"": ""Me"", ""picture"": { ""data"": { ""height"": 50, ""is_silhouette"": false, ""url"": ""https://example.com/p.jpg"", ""width"": 50 } }, ""verified"": true}";

        var handler = new TestHttpMessageHandler(req =>
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(profileJson)
            };
            return resp;
        });

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://graph.facebook.com/v18.0/")
        };

        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var utilities = new FacebookClientUtilities(opts);
        var logger = Mock.Of<ILogger<FacebookHttpClient>>();

        var jsonOpts = CreateJsonOptionsWithConverter();

        var fb = new FacebookHttpClient(client, utilities, opts, logger, jsonOpts);

        // Act
        var result = await fb.GetUserProfileAsync(userToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe("101");
        result.Value.Email.ShouldBe("me@example.com");
        result.Value.Picture.ShouldNotBeNull();
        result.Value.Picture.Data?.Url.ShouldBe("https://example.com/p.jpg");
    }

    //----------------------//

    [Fact]
    public async Task GetDebugTokenAsync_ReturnsFailure_WhenResponseIsNon200()
    {
        // Arrange
        var userToken = "some_user_token";
        var handler = new TestHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad request") };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var utilities = new FacebookClientUtilities(opts);
        var logger = Mock.Of<ILogger<FacebookHttpClient>>();
        var jsonOpts = CreateJsonOptionsWithConverter();
        var fb = new FacebookHttpClient(client, utilities, opts, logger, jsonOpts);

        // Act
        var result = await fb.GetDebugTokenAsync(userToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Failed to retrieve debug token", Case.Insensitive);
        // New assertions: ensure status code and body are included in the Info
        result.Info.ShouldContain("StatusCode: 400");
        result.Info.ShouldContain("bad request");
    }

    //----------------------//

    [Fact]
    public async Task GetUserProfileAsync_ReturnsFailure_WhenResponseIsNon200()
    {
        // Arrange
        var userToken = "some_user_token";
        var handler = new TestHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("oops") };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var utilities = new FacebookClientUtilities(opts);
        var logger = Mock.Of<ILogger<FacebookHttpClient>>();
        var jsonOpts = CreateJsonOptionsWithConverter();
        var fb = new FacebookHttpClient(client, utilities, opts, logger, jsonOpts);

        // Act
        var result = await fb.GetUserProfileAsync(userToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Failed to retrieve user profile", Case.Insensitive);
        // New assertions: ensure status code and body are included in the Info
        result.Info.ShouldContain("StatusCode: 500");
        result.Info.ShouldContain("oops");
    }

    [Fact]
    public async Task GetDebugTokenAsync_ReturnsFailure_WhenResponseIsNon200_And_LogsWarning()
    {
        // Arrange
        var userToken = "some_user_token";
        var handler = new TestHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad request") };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var utilities = new FacebookClientUtilities(opts);
        var mockLogger = new Mock<ILogger<FacebookHttpClient>>();
        var jsonOpts = CreateJsonOptionsWithConverter();
        var fb = new FacebookHttpClient(client, utilities, opts, mockLogger.Object, jsonOpts);

        // Act
        var result = await fb.GetDebugTokenAsync(userToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("StatusCode: 400");
        result.Info.ShouldContain("bad request");

        // Verify warning logged
        mockLogger.VerifyWarningLogging(msg => msg.ToString()?.Contains("debug_token request failed") == true, Times.Once);
    }

    //----------------------//

    [Fact]
    public async Task GetUserProfileAsync_ReturnsFailure_WhenResponseIsNon200_And_LogsWarning()
    {
        // Arrange
        var userToken = "some_user_token";
        var handler = new TestHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("oops") };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var utilities = new FacebookClientUtilities(opts);
        var mockLogger = new Mock<ILogger<FacebookHttpClient>>();
        var jsonOpts = CreateJsonOptionsWithConverter();
        var fb = new FacebookHttpClient(client, utilities, opts, mockLogger.Object, jsonOpts);

        // Act
        var result = await fb.GetUserProfileAsync(userToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("StatusCode: 500");
        result.Info.ShouldContain("oops");

        // Verify warning logged
        mockLogger.VerifyWarningLogging(msg => msg.ToString()?.Contains("/me request failed") == true, Times.Once);
    }

    //----------------------//

    [Fact]
    public async Task GetDebugTokenAsync_DeserializationFailure_LogsWarningAndReturnsFailure()
    {
        // Arrange: return invalid JSON that will cause deserialization to throw
        var handler = new TestHttpMessageHandler(req =>
        {
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{notjson}") };
        });

        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var utilities = new FacebookClientUtilities(opts);
        var mockLogger = new Mock<ILogger<FacebookHttpClient>>();
        var jsonOpts = CreateJsonOptionsWithConverter();
        var fb = new FacebookHttpClient(client, utilities, opts, mockLogger.Object, jsonOpts);

        // Act
        var result = await fb.GetDebugTokenAsync("token");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Failed to parse debug token response");

        mockLogger.VerifyWarningLogging(msg => msg.ToString()?.Contains("Failed to deserialize debug_token response") == true, Times.Once);
    }
}
