using ID.Application.Middleware;
using ID.Domain.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using System.Security.Claims;

namespace ID.Application.Tests.Middlleware;

public class MultiFactorRequiredMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly MultiFactorRequiredMiddleware _middleware;

    public MultiFactorRequiredMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new MultiFactorRequiredMiddleware(_nextMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenMfaIsNotUsed()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextMock.Verify(next => next(context), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task InvokeAsync_ShouldReturnUnauthorized_WhenTwoFactorRequiredAndNotVerified()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var claims = new List<Claim>
    {
        TwoFactorClaims.TwoFactorRequired,
        TwoFactorClaims.TwoFactor_NOT_Verified
    };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        context.User = new ClaimsPrincipal(identity);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        _nextMock.Verify(next => next(context), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenTwoFactorNotRequired()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            //TwoFactorRequired is missing
            TwoFactorClaims.TwoFactor_NOT_Verified
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        context.User = new ClaimsPrincipal(identity);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextMock.Verify(next => next(context), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenTwoFactorVerified()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var claims = new List<Claim>
            {
                TwoFactorClaims.TwoFactorRequired,
                TwoFactorClaims.TwoFactorVerified
            };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        context.User = new ClaimsPrincipal(identity);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _nextMock.Verify(next => next(context), Times.Once);
    }

    //------------------------------------//

}//Cls