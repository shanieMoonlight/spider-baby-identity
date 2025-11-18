//using ID.GlobalSettings.Setup.Options;
//using ID.OAuth.Facebook.HttpService.Imps;
//using ID.OAuth.Utils.Abs;
//using ID.OAuth.Utils.Imps;

//namespace ID.OAuth.Facebook.Tests;

//public class FacebookClientRateLimitTests
//{
//    private static JsonSerializerOptions CreateJsonOptions() => new JsonSerializerOptions
//    {
//        PropertyNameCaseInsensitive = true,
//        AllowTrailingCommas = true,
//        ReadCommentHandling = JsonCommentHandling.Skip
//    };

//    [Fact]
//    public async Task GetDebugTokenAsync_ReturnsRateLimitResult_When429()
//    {
//        // Arrange
//        var handler = new TestHttpMessageHandler(req =>
//        {
//            return new HttpResponseMessage(HttpStatusCode.TooManyRequests) { Content = new StringContent("too many") };
//        });

//        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
//        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
//        var fbUtilities = new FacebookClientUtilities(opts);
//        Mock<IOAuthHttpClientUtils> _mockGenUtils = new();


//        var logger = Mock.Of<ILogger<FacebookHttpClient>>();
//        var jsonOpts = CreateJsonOptions();

//        var fb = new FacebookHttpClient(client, fbUtilities, _mockGenUtils.Object,  opts, logger, jsonOpts);

//        // Act
//        var result = await fb.GetDebugTokenAsync("token");

//        // Assert
//        result.Succeeded.ShouldBeFalse();
//        result.Status.ShouldBe(BasicResult.ResultStatus.RateLimitExceeded);
//        result.Info.ShouldContain("rate_limited", Case.Insensitive);
//    }

//    [Fact]
//    public async Task GetDebugTokenAsync_DoesNotRetry_On429()
//    {
//        // Arrange
//        var calls = 0;
//        var handler = new TestHttpMessageHandler(req =>
//        {
//            calls++;
//            if (calls == 1)
//                return new HttpResponseMessage(HttpStatusCode.TooManyRequests) { Content = new StringContent("too many") };
//            // would succeed on second call if retried
//            var debugJson = "{\"data\":{\"app_id\":\"app123\",\"is_valid\":true,\"user_id\":\"user_1\",\"expires_at\":9999999999}}";
//            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(debugJson) };
//        });

//        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
//        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
//        var utilities = new FacebookClientUtilities(opts);
//        var logger = Mock.Of<ILogger<FacebookHttpClient>>();
//        var jsonOpts = CreateJsonOptions();

//        var fb = new FacebookHttpClient(client, utilities, opts, logger, jsonOpts);

//        // Act
//        var result = await fb.GetDebugTokenAsync("token");

//        // Assert - ensure only one call was made (no retry) and rate-limit result returned
//        calls.ShouldBe(1);
//        result.Succeeded.ShouldBeFalse();
//        result.Status.ShouldBe(BasicResult.ResultStatus.RateLimitExceeded);
//    }

//    [Fact]
//    public async Task GetUserProfileAsync_ReturnsRateLimitResult_When429()
//    {
//        // Arrange
//        var handler = new TestHttpMessageHandler(req =>
//        {
//            return new HttpResponseMessage(HttpStatusCode.TooManyRequests) { Content = new StringContent("too many") };
//        });

//        var client = new HttpClient(handler) { BaseAddress = new Uri("https://graph.facebook.com/v18.0/") };
//        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
//        var utilities = new FacebookClientUtilities(opts);
//        var logger = Mock.Of<ILogger<FacebookHttpClient>>();
//        var jsonOpts = CreateJsonOptions();

//        var fb = new FacebookHttpClient(client, utilities, opts, logger, jsonOpts);

//        // Act
//        var result = await fb.GetUserProfileAsync("token");

//        // Assert
//        result.Succeeded.ShouldBeFalse();
//        result.Status.ShouldBe(BasicResult.ResultStatus.RateLimitExceeded);
//        result.Info.ShouldContain("rate_limited", Case.Insensitive);
//    }
//}
