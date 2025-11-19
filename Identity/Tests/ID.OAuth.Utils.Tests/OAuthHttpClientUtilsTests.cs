using ID.OAuth.Utils.Services.Imps;

namespace ID.OAuth.Utils.Tests;

public class OAuthHttpClientUtilsTests
{
    private static OAuthHttpClientUtils CreateSut()
    {
        var logger = Mock.Of<ILogger<OAuthHttpClientUtils>>();
        return new OAuthHttpClientUtils(logger);
    }

    //---------------------//

    [Fact]
    public void MapResponseToResult_ReturnsUnauthorized_When401()
    {
        var sut = CreateSut();
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("no auth") };

        var result = sut.MapResponseToResult<string>(response, "Prov", "/endpoint", "body");

        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.Unauthorized);
    }

    //---------------------//

    [Fact]
    public void MapResponseToResult_ReturnsForbidden_When403()
    {
        var sut = CreateSut();
        var response = new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent("forbidden") };

        var result = sut.MapResponseToResult<string>(response, "Prov", "/endpoint", "body");

        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.Forbidden);
    }

    //---------------------//

    [Fact]
    public void MapResponseToResult_ReturnsRateLimit_When429()
    {
        var sut = CreateSut();
        var response = new HttpResponseMessage((HttpStatusCode)429) { Content = new StringContent("too many") };

        var result = sut.MapResponseToResult<string>(response, "Prov", "/endpoint", "body");

        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.RateLimitExceeded);
        result.Info.ShouldContain("rate_limited");
    }

    //---------------------//

    [Fact]
    public void MapResponseToResult_ReturnsBadRequest_When400()
    {
        var sut = CreateSut();
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("bad") };

        var result = sut.MapResponseToResult<string>(response, "Prov", "/endpoint", "body");

        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.BadRequest);
    }

    //---------------------//

    [Fact]
    public void MapResponseToResult_ReturnsFailure_When500()
    {
        var sut = CreateSut();
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("err") };

        var result = sut.MapResponseToResult<string>(response, "Prov", "/endpoint", "body");

        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.Failure);
    }
}
