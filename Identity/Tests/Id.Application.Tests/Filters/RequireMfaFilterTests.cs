using ID.Application.Authenticators;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ID.Application.Tests.Filters;

public class RequireMfaFilterTests
{
    [Fact]
    public void OnActionExecuting_ShouldReturnUnauthorized_WhenTwoFactorNotVerified()
    {
        // Arrange
        var filter = new MfaVerifiedAuthenticator.ActionFilter();
        var context = new ActionExecutingContext(
            ContextProvider.GetActionContext(),
            [],
            new Dictionary<string, object?>(),
            new object()
        );

        // Act
        filter.OnActionExecuting(context);

        // Assert
        Assert.IsType<ForbidResult>(context.Result);
    }

    //-------------------------------------//

    [Fact]
    public void OnActionExecuting_ShouldProceed_WhenTwoFactorVerified()
    {
        // Arrange
        var filter = new MfaVerifiedAuthenticator.ActionFilter();
        var context = new ActionExecutingContext(
            ContextProvider.GetActionContextWithTwoFactorVerified(),
            [],
            new Dictionary<string, object?>(),
            new object()
        );

        // Act
        filter.OnActionExecuting(context);

        // Assert
        Assert.Null(context.Result);
    }

    //-------------------------------------//

}//Cls
