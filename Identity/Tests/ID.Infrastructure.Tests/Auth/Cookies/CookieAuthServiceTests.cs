using ID.Application.MFA;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.Auth.Cookies;
using ID.Infrastructure.Auth.Cookies.Services;
using ID.Infrastructure.Claims.Services.Abs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace ID.Infrastructure.Tests.Auth.Cookies;

public class CookieAuthServiceTests
{
    private readonly Mock<IClaimsBuilderService> _claimsBuilderMock = new();
    private readonly Mock<ITwofactorUserIdCacheService> _twofactorUserIdCacheMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly Mock<HttpRequest> _httpRequestMock = new();
    private readonly Mock<HttpResponse> _httpResponseMock = new();
    private readonly Mock<IResponseCookies> _responseCookiesMock = new();
    private readonly Mock<IRequestCookieCollection> _requestCookiesMock = new();

    private readonly AppUser _user = AppUserDataFactory.Create();
    private readonly Team _team = TeamDataFactory.Create();

    public CookieAuthServiceTests()
    {
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);
        _httpContextMock.Setup(x => x.Request).Returns(_httpRequestMock.Object);
        _httpContextMock.Setup(x => x.Response).Returns(_httpResponseMock.Object);
        _httpRequestMock.Setup(x => x.Cookies).Returns(_requestCookiesMock.Object);
        _httpResponseMock.Setup(x => x.Cookies).Returns(_responseCookiesMock.Object);
    }


    //-----------------------------//

    [Fact]
    public async Task CreateWithTwoFactorRequiredAsync_Should_Set2FACookies()
    {
        // Arrange
        _twofactorUserIdCacheMock.Setup(x => x.StoreUserId(_user.Id)).Returns("token123");
        _httpRequestMock.Setup(x => x.IsHttps).Returns(true);
        var authService = new CookieAuthService<AppUser>(
            _claimsBuilderMock.Object,
            _twofactorUserIdCacheMock.Object,
            _httpContextAccessorMock.Object);

        // Act
        await authService.CreateWithTwoFactorRequiredAsync(true, _user, CookieConstants.DeviceIdKey);

        // Assert
        _responseCookiesMock.Verify(x => x.Append(CookieConstants.TwoFactorTokenKey, "token123", It.IsAny<Microsoft.AspNetCore.Http.CookieOptions>()), Times.Once);
        _responseCookiesMock.Verify(x => x.Append(CookieConstants.IsPersistentKey, "True", It.IsAny<Microsoft.AspNetCore.Http.CookieOptions>()), Times.Once);
        _responseCookiesMock.Verify(x => x.Append(CookieConstants.DeviceIdKey, CookieConstants.DeviceIdKey, It.IsAny<Microsoft.AspNetCore.Http.CookieOptions>()), Times.Once);
    }

    //-----------------------------//

    [Fact]
    public void TryGetTwoFactorToken_Should_ReturnTokenFromCookie()
    {
        // Arrange
        _requestCookiesMock.Setup(x => x.TryGetValue(CookieConstants.TwoFactorTokenKey, out It.Ref<string?>.IsAny))
            .Callback(new TryGetValueCallback((string key, out string value) => value = "token123"))
            .Returns(true);
        var authService = new CookieAuthService<AppUser>(
            _claimsBuilderMock.Object,
            _twofactorUserIdCacheMock.Object,
            _httpContextAccessorMock.Object);

        // Act
        var result = authService.TryGetTwoFactorToken();

        // Assert
        result.ShouldBe("token123");
    }

    //-----------------------------//

    [Fact]
    public void GetRememberMe_Should_ReturnParsedBool()
    {
        // Arrange
        _requestCookiesMock.Setup(x => x.TryGetValue(CookieConstants.IsPersistentKey, out It.Ref<string?>.IsAny))
            .Callback(new TryGetValueCallback((string key, out string value) => value = "true"))
            .Returns(true);
        var authService = new CookieAuthService<AppUser>(
            _claimsBuilderMock.Object,
            _twofactorUserIdCacheMock.Object,
            _httpContextAccessorMock.Object);

        // Act
        var result = authService.GetRememberMe();

        // Assert
        result.ShouldBeTrue();
    }

    //-----------------------------//

    [Fact]
    public void TryGetDeviceId_Should_ReturnDeviceIdFromCookie()
    {
        // Arrange
        _requestCookiesMock.Setup(x => x.TryGetValue(CookieConstants.DeviceIdKey, out It.Ref<string?>.IsAny))
            .Callback(new TryGetValueCallback((string key, out string value) => value = "deviceId123"))
            .Returns(true);
        var authService = new CookieAuthService<AppUser>(
            _claimsBuilderMock.Object,
            _twofactorUserIdCacheMock.Object,
            _httpContextAccessorMock.Object);

        // Act
        var result = authService.TryGetDeviceId();

        // Assert
        result.ShouldBe("deviceId123");
    }

    //-----------------------------//

    // Helper for TryGetValue callback
    private delegate void TryGetValueCallback(string key, out string value);

}//Cls
