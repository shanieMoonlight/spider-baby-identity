using ID.OAuth.Amazon.HttpService.Abs;
using ID.OAuth.Amazon.Services.Imps;

namespace ID.OAuth.Amazon.Tests.Services;

public class AmazonAuthenticationServiceTests
{
    [Fact]
    public async Task VerifyTokenAsync_ReturnsSuccess_WhenTokenValid()
    {
        // Arrange
        var tokenInfo = new AmazonTokenInfo { ClientId = "cid", UserId = "uid", ExpiresIn = 1000 };
        var mockHttp = new Mock<IAmazonHttpClient>();
        mockHttp.Setup(h => h.GetTokenInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonTokenInfo>.Success(tokenInfo));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        // Act
        var res = await svc.VerifyTokenAsync("token", "uid");

        // Assert
        res.Succeeded.ShouldBeTrue();
        res.Value.ShouldBe(tokenInfo);
    }

    //--------------------------//

    [Fact]
    public async Task VerifyTokenAsync_ReturnsUnauthorized_WhenClientIdMismatch()
    {
        // Arrange
        var tokenInfo = new AmazonTokenInfo { ClientId = "other", UserId = "uid", ExpiresIn = 1000 };
        var mockHttp = new Mock<IAmazonHttpClient>();
        mockHttp.Setup(h => h.GetTokenInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonTokenInfo>.Success(tokenInfo));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        // Act
        var res = await svc.VerifyTokenAsync("token", "uid");

        // Assert
        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.Unauthorized);
    }

    //--------------------------//

    [Fact]
    public async Task VerifyTokenAsync_ReturnsUnauthorized_WhenExpired()
    {
        // Arrange
        var tokenInfo = new AmazonTokenInfo { ClientId = "cid", UserId = "uid", ExpiresIn = -10 };
        var mockHttp = new Mock<IAmazonHttpClient>();
        mockHttp.Setup(h => h.GetTokenInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonTokenInfo>.Success(tokenInfo));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        // Act
        var res = await svc.VerifyTokenAsync("token", "uid");

        // Assert
        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.Unauthorized);
    }

    //--------------------------//

    [Fact]
    public async Task VerifyTokenAsync_ReturnsUnauthorized_WhenUserIdMismatch()
    {
        // Arrange
        var tokenInfo = new AmazonTokenInfo { ClientId = "cid", UserId = "other", ExpiresIn = 1000 };
        var mockHttp = new Mock<IAmazonHttpClient>();
        mockHttp.Setup(h => h.GetTokenInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonTokenInfo>.Success(tokenInfo));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        // Act
        var res = await svc.VerifyTokenAsync("token", "expected");

        // Assert
        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.Unauthorized);
    }

    //--------------------------//

    [Fact]
    public async Task VerifyTokenAsync_PropagatesRateLimit_FromHttpClient()
    {
        // Arrange
        var mockHttp = new Mock<IAmazonHttpClient>();
        mockHttp.Setup(h => h.GetTokenInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonTokenInfo>.RateLimitExceededResult("rate_limited"));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        // Act
        var res = await svc.VerifyTokenAsync("token", string.Empty);

        // Assert
        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.RateLimitExceeded);
    }

    //--------------------------//

    [Fact]
    public async Task VerifyAndGetProfileAsync_ReturnsProfile_WhenVerifyAndProfileSucceed()
    {
        // Arrange
        var tokenInfo = new AmazonTokenInfo { ClientId = "cid", UserId = "uid", ExpiresIn = 1000 };
        var profile = new AmazonUserProfile { UserId = "uid", Email = "me@example.com", Name = "Me" };

        var mockHttp = new Mock<IAmazonHttpClient>();
        mockHttp.Setup(h => h.GetTokenInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonTokenInfo>.Success(tokenInfo));
        mockHttp.Setup(h => h.GetUserProfileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonUserProfile>.Success(profile));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        // Act
        var res = await svc.VerifyAndGetProfileAsync("token", "uid");

        // Assert
        res.Succeeded.ShouldBeTrue();
        res.Value.ShouldBe(profile);
    }

    //--------------------------//

    [Fact]
    public async Task VerifyAndGetProfileAsync_ReturnsUnauthorized_WhenProfileUserIdMismatch()
    {
        // Arrange
        var tokenInfo = new AmazonTokenInfo { ClientId = "cid", UserId = "uid", ExpiresIn = 1000 };
        var profile = new AmazonUserProfile { UserId = "other", Email = "me@example.com", Name = "Me" };

        var mockHttp = new Mock<IAmazonHttpClient>();
        mockHttp.Setup(h => h.GetTokenInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonTokenInfo>.Success(tokenInfo));
        mockHttp.Setup(h => h.GetUserProfileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonUserProfile>.Success(profile));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        // Act
        var res = await svc.VerifyAndGetProfileAsync("token", "uid");

        // Assert
        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.Unauthorized);
    }

    //--------------------------//

    // New tests added below

    [Fact]
    public async Task VerifyTokenAsync_ReturnsBadRequest_WhenEmptyToken()
    {
        var mockHttp = new Mock<IAmazonHttpClient>();
        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        var res = await svc.VerifyTokenAsync(string.Empty, string.Empty);

        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.BadRequest);
    }

    //---------------------//

    [Fact]
    public async Task VerifyTokenAsync_ReturnsFailure_WhenMissingServerClientId()
    {
        var mockHttp = new Mock<IAmazonHttpClient>();
        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = string.Empty });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        var res = await svc.VerifyTokenAsync("token", string.Empty);

        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.Failure);
        res.Info.ShouldContain("missing_server_credentials");
    }

    //---------------------//

    [Fact]
    public async Task GetUserProfileAsync_ReturnsBadRequest_WhenEmptyToken()
    {
        var mockHttp = new Mock<IAmazonHttpClient>();
        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        var res = await svc.GetUserProfileAsync(string.Empty);

        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.BadRequest);
    }

    //---------------------//

    [Fact]
    public async Task GetUserProfileAsync_ReturnsFailure_WhenProfileMissingUserId()
    {
        var profile = new AmazonUserProfile { UserId = string.Empty, Email = "me@example.com", Name = "Me" };
        var mockHttp = new Mock<IAmazonHttpClient>();
        mockHttp.Setup(h => h.GetUserProfileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonUserProfile>.Success(profile));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        var res = await svc.GetUserProfileAsync("token");

        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.Failure);
    }

    //---------------------//

    [Fact]
    public async Task VerifyAndGetProfileAsync_PropagatesTokenVerifyFailure_WhenTokenInvalid()
    {
        var mockHttp = new Mock<IAmazonHttpClient>();
        mockHttp.Setup(h => h.GetTokenInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonTokenInfo>.UnauthorizedResult("nope"));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        var res = await svc.VerifyAndGetProfileAsync("token");

        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task VerifyAndGetProfileAsync_PropagatesProfileFailure_WhenProfileCallFails()
    {
        var tokenInfo = new AmazonTokenInfo { ClientId = "cid", UserId = "uid", ExpiresIn = 1000 };
        var mockHttp = new Mock<IAmazonHttpClient>();
        mockHttp.Setup(h => h.GetTokenInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonTokenInfo>.Success(tokenInfo));
        mockHttp.Setup(h => h.GetUserProfileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonUserProfile>.RateLimitExceededResult("rate_limited"));

        var opts = Options.Create(new IdOAuthAmazonOptions { ClientId = "cid" });
        var svc = new AmazonAuthenticationService(mockHttp.Object, opts, Mock.Of<ILogger<AmazonAuthenticationService>>());

        var res = await svc.VerifyAndGetProfileAsync("token");

        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.RateLimitExceeded);
    }

}//Cls
