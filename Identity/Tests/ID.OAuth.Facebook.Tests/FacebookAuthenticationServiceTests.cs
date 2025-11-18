using ID.OAuth.Facebook.HttpService.Abs;
using ID.OAuth.Facebook.Services.Imps;
using MyResults;

namespace ID.OAuth.Facebook.Tests;

public class FacebookAuthenticationServiceTests
{
    [Fact]
    public async Task VerifyAndGetProfileAsync_ReturnsProfile_WhenDebugAndProfileMatch()
    {
        // Arrange
        var userToken = "valid_user_token";
        var userId = "user_123";

        var debug = new FacebookDebugTokenData
        {
            AppId = "app123",
            IsValid = true,
            UserId = userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        };

        var profile = new FacebookUserProfile
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User"
        };

        var mockHttp = new Mock<IFacebookHttpClient>();
        mockHttp.Setup(x => x.GetDebugTokenAsync(userToken, default))
            .ReturnsAsync(GenResult<FacebookDebugTokenData>.Success(debug));
        mockHttp.Setup(x => x.GetUserProfileAsync(userToken, default))
            .ReturnsAsync(GenResult<FacebookUserProfile>.Success(profile));

        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var logger = Mock.Of<ILogger<FacebookAuthenticationService>>();

        var service = new FacebookAuthenticationService(mockHttp.Object, opts, logger);

        // Act
        var result = await service.VerifyAndGetProfileAsync(userToken, userId);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(userId);
    }

    //----------------------//

    [Fact]
    public async Task VerifyAndGetProfileAsync_Fails_WhenDebugTokenInvalid()
    {
        // Arrange
        var userToken = "invalid_token";
        var userId = "user_123";

        var debug = new FacebookDebugTokenData
        {
            AppId = "app123",
            IsValid = false,
            UserId = userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        };

        var mockHttp = new Mock<IFacebookHttpClient>();
        mockHttp.Setup(x => x.GetDebugTokenAsync(userToken, default))
            .ReturnsAsync(GenResult<FacebookDebugTokenData>.Success(debug));

        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var logger = Mock.Of<ILogger<FacebookAuthenticationService>>();

        var service = new FacebookAuthenticationService(mockHttp.Object, opts, logger);

        // Act
        var result = await service.VerifyAndGetProfileAsync(userToken, userId);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.Unauthorized);
        result.Info.ShouldContain("invalid", Case.Insensitive);
    }

    //----------------------//

    [Fact]
    public async Task VerifyAndGetProfileAsync_Fails_WhenProfileIdMismatch()
    {
        // Arrange
        var userToken = "valid_user_token";
        var debugUserId = "user_123";

        var debug = new FacebookDebugTokenData
        {
            AppId = "app123",
            IsValid = true,
            UserId = debugUserId,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
        };

        var profile = new FacebookUserProfile
        {
            Id = "different_user",
            Email = "test@example.com",
            Name = "Test User"
        };

        var mockHttp = new Mock<IFacebookHttpClient>();
        mockHttp.Setup(x => x.GetDebugTokenAsync(userToken, default))
            .ReturnsAsync(GenResult<FacebookDebugTokenData>.Success(debug));
        mockHttp.Setup(x => x.GetUserProfileAsync(userToken, default))
            .ReturnsAsync(GenResult<FacebookUserProfile>.Success(profile));

        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var logger = Mock.Of<ILogger<FacebookAuthenticationService>>();

        var service = new FacebookAuthenticationService(mockHttp.Object, opts, logger);

        // Act
        var result = await service.VerifyAndGetProfileAsync(userToken, debugUserId);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.Unauthorized);
        result.Info.ShouldContain("match", Case.Insensitive);
    }

    //----------------------//

    [Fact]
    public async Task VerifyAndGetProfileAsync_Fails_WhenDebugTokenExpired()
    {
        // Arrange
        var userToken = "expired_token";
        var userId = "user_123";

        var debug = new FacebookDebugTokenData
        {
            AppId = "app123",
            IsValid = true,
            UserId = userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1)
        };

        var mockHttp = new Mock<IFacebookHttpClient>();
        mockHttp.Setup(x => x.GetDebugTokenAsync(userToken, default))
            .ReturnsAsync(GenResult<FacebookDebugTokenData>.Success(debug));

        var opts = Options.Create(new IdOAuthFacebookOptions { AppId = "app123", AppSecret = "secret" });
        var logger = Mock.Of<ILogger<FacebookAuthenticationService>>();

        var service = new FacebookAuthenticationService(mockHttp.Object, opts, logger);

        // Act
        var result = await service.VerifyAndGetProfileAsync(userToken, userId);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.Unauthorized);
        result.Info.ShouldContain("expired", Case.Insensitive);
    }
}//Cls
