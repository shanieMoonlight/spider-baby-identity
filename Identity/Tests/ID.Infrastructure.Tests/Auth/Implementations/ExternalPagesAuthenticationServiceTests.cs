using ID.Domain.Claims.Utils;
using ID.GlobalSettings.Constants;
using ID.Infrastructure.Auth.AppAbs;
using ID.Infrastructure.Auth.AppImps;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Persistance.EF.Setup.Options;
using ID.Infrastructure.Setup;
using ID.Infrastructure.Setup.Options;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ID.Infrastructure.Tests.Auth.Implementations;

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
public class ExternalPagesAuthenticationServiceTests
{
    private readonly Mock<IExternalPageListService> _externalPageListMock = new();
    private readonly Mock<IHostEnvironment> _envMock = new();
    private readonly Mock<IOptions<IdInfrastructureOptions>> _optionsMock = new();
    private readonly DefaultHttpContext _httpContext = new();

    private ExternalPageAuthenticationService CreateService()
        => new(_externalPageListMock.Object, _optionsMock.Object, _envMock.Object);

    //- - - - - - - - - - - - - - - - - - -//

    [Fact]
    public async Task ReturnsNoResult_WhenNotFromExternalPage()
    {
        _externalPageListMock.Setup(x => x.IsFromExternalPage(It.IsAny<HttpRequest>())).Returns(false);

        var service = CreateService();
        var result = await service.TryAuthenticateAsync(_httpContext);

        result.None.ShouldBeTrue();
    }

    //- - - - - - - - - - - - - - - - - - -//

    [Fact]
    public async Task ReturnsCookieResult_WhenCookieAuthenticationSucceeds()
    {
        _externalPageListMock.Setup(x => x.IsFromExternalPage(It.IsAny<HttpRequest>())).Returns(true);

        var expectedResult = AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), CookieAuthenticationDefaults.AuthenticationScheme));
        _httpContext.Features.Set<IAuthenticateResultFeature>(new TestAuthResultFeature(expectedResult));

        _httpContext.RequestServices = new TestAuthServiceProvider(expectedResult);

        var service = CreateService();
        var result = await service.TryAuthenticateAsync(_httpContext);

        result.Succeeded.ShouldBeTrue();
    }

    //- - - - - - - - - - - - - - - - - - -//

    [Fact]
    public async Task AuthenticatesWithJwt_WhenJwtQueryPresent()
    {
        _externalPageListMock.Setup(x => x.IsFromExternalPage(It.IsAny<HttpRequest>())).Returns(true);

        var jwtHandler = new JwtSecurityTokenHandler();
        var token = jwtHandler.CreateEncodedJwt(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(MyIdClaimTypes.NAME, "jwtuser")]),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = null // Not validated in ReadJwtToken
        });

        _httpContext.Request.QueryString = new QueryString($"?{IdGlobalConstants.ExtraAuth.JWT_QUERY_KEY}={token}");

        // Mock extension methods
        _httpContext.Request.Headers["CacheControl"] = "";
        _httpContext.Request.Headers["Pragma"] = "";

        // Add this line to fix the error:
        _httpContext.RequestServices = new TestAuthServiceProvider(AuthenticateResult.NoResult());

        var service = CreateService();
        var result = await service.TryAuthenticateAsync(_httpContext);

        result.Succeeded.ShouldBeTrue();
        result.Principal.Claims.ShouldContain(
            c => c.Type == MyIdClaimTypes.NAME && c.Value == "jwtuser"
        );
    }

    //- - - - - - - - - - - - - - - - - - -//

    [Fact]
    public async Task AuthenticatesAsDevUser_WhenDevModeEnabled()
    {
        _externalPageListMock.Setup(x => x.IsFromExternalPage(It.IsAny<HttpRequest>())).Returns(true);
        _envMock.SetupGet(x => x.EnvironmentName).Returns("Development");


        var options = new IdInfrastructureOptions
        {
            // Providing a non-empty symmetric key makes UseAsymmetricCrypto return false
            AllowExternalPagesDevModeAccess = true
        };

        _optionsMock.Setup(o => o.Value).Returns(options);

        _httpContext.RequestServices = new TestAuthServiceProvider(AuthenticateResult.NoResult());
        var service = CreateService();
        var result = await service.TryAuthenticateAsync(_httpContext);

        result.Succeeded.ShouldBeTrue();


        result.Principal.Identity?.Name.ShouldBe("DevUser");
    }

    //- - - - - - - - - - - - - - - - - - -//

    [Fact]
    public async Task ReturnsNoResult_WhenNoOtherConditionMet()
    {
        _externalPageListMock.Setup(x => x.IsFromExternalPage(It.IsAny<HttpRequest>())).Returns(true);
        _envMock.SetupGet(x => x.EnvironmentName).Returns("Production");
        _httpContext.RequestServices = new TestAuthServiceProvider(AuthenticateResult.NoResult());

        var service = CreateService();
        var result = await service.TryAuthenticateAsync(_httpContext);

        result.None.ShouldBeTrue();
    }

    //- - - - - - - - - - - - - - - - - - -//

    [Fact]
    public async Task ReturnsNoResult_WhenExceptionIsThrown()
    {
        // Arrange
        // Setup mock to throw an exception when IsFromExternalPage is called
        _externalPageListMock.Setup(x => x.IsFromExternalPage(It.IsAny<HttpRequest>()))
            .Throws(new InvalidOperationException("Test exception"));

        var service = CreateService();

        // Act
        var result = await service.TryAuthenticateAsync(_httpContext);

        // Assert
        result.None.ShouldBeTrue("Should return NoResult when an exception is thrown");
        result.Succeeded.ShouldBeFalse("Should not be successful when an exception is thrown");
    }

    //- - - - - - - - - - - - - - - - - - -//

    // Helpers for mocking authentication
    private class TestAuthResultFeature(AuthenticateResult result) : IAuthenticateResultFeature
    {
        public AuthenticateResult AuthenticateResult { get; set; } = result;
    }

    //- - - - - - - - - - - - - - - - - - -//

    private class TestAuthServiceProvider(AuthenticateResult result) : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IAuthenticationService))
                return new TestAuthenticationService(result);
            return null!;
        }
    }

    //- - - - - - - - - - - - - - - - - - -//

    private class TestAuthenticationService(AuthenticateResult result) : IAuthenticationService
    {
        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme) => Task.FromResult(result);
        public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties) => Task.CompletedTask;
        public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties) => Task.CompletedTask;
        public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties) => Task.CompletedTask;
        public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties) => Task.CompletedTask;
    }

    //- - - - - - - - - - - - - - - - - - -//

}//Cls

#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).