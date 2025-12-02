using ID.Application.Authenticators;
using System.IdentityModel.Tokens.Jwt;

namespace ID.Application.Tests.Authenticators;

public class CanTrustDeviceAuthenticatorTests
{
    [Fact]
    public Task IsAuthorized_ReturnsTrue_WhenAuthenticatedWithMfaAndAuthTimeWithinLimit()
    {
        // Arrange
        var authTime = DateTime.UtcNow.AddMinutes(-1);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Amr, AuthMethodRef.mfa.ToString()),
            AuthenticationClaims.AuthTime(authTime)
        };
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var handler = new CanTrustDeviceAuthenticator.AuthHandler();

        // Act
        var result = handler.IsAuthorized(httpContext);

        // Assert
        result.ShouldBeTrue();
        return Task.CompletedTask;
    }

    //--------------------//

    [Fact]
    public Task IsAuthorized_ReturnsFalse_WhenNotAuthenticated()
    {
        var claims = new List<Claim>();
        var httpContext = ContextProvider.GetHttpContext(claims, false);
        var handler = new CanTrustDeviceAuthenticator.AuthHandler();

        var result = handler.IsAuthorized(httpContext);

        result.ShouldBeFalse();
        return Task.CompletedTask;
    }

    //--------------------//

    [Fact]
    public Task IsAuthorized_ReturnsFalse_WhenMissingMfaOrOauth()
    {
        var authTime = DateTime.UtcNow.AddMinutes(-5);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Amr, AuthMethodRef.pwd.ToString()),
            AuthenticationClaims.AuthTime(authTime)
        };
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var handler = new CanTrustDeviceAuthenticator.AuthHandler();

        var result = handler.IsAuthorized(httpContext);

        result.ShouldBeFalse();
        return Task.CompletedTask;
    }

    //--------------------//

    [Fact]
    public Task IsAuthorized_ReturnsFalse_WhenAuthTimeTooOld()
    {
        var authTime = DateTime.UtcNow.AddMinutes(-100);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Amr, AuthMethodRef.mfa.ToString()),
            AuthenticationClaims.AuthTime(authTime)
        };
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var handler = new CanTrustDeviceAuthenticator.AuthHandler();

        var result = handler.IsAuthorized(httpContext);

        result.ShouldBeFalse();
        return Task.CompletedTask;
    }
}
