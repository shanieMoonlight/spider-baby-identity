using ID.Application.Authenticators;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ID.Application.Tests.Filters;

public class InitializerFilterAttributeTests
{
    [Fact]
    public void OnActionExecuting_ShouldReturnNotFound_WhenAlreadyInitialized()
    {
        // Arrange
        var filter = new InitializedAuthenticator.ActionFilter();
        var context = new ActionExecutingContext(
            ContextProvider.GetActionContextWithInitializedTeam(),
            [],
            new Dictionary<string, object?>(),
            new object()
        );

        // Act
        filter.OnActionExecuting(context);

        // Assert
        Assert.IsType<NotFoundResult>(context.Result);
    }

    //-------------------------------------//

    [Fact]
    public void OnActionExecuting_ShouldProceed_WhenNotInitialized()
    {
        // Arrange
        var filter = new InitializedAuthenticator.ActionFilter();
        var context = new ActionExecutingContext(
            ContextProvider.GetActionContextWithUninitializedTeam(),
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

    [Fact]
    public void OnActionExecuting_ShouldProceed_WhenSuperLeader()
    {
        // Arrange
        var filter = new InitializedAuthenticator.ActionFilter();
        var context = new ActionExecutingContext(
            ContextProvider.GetActionContextWithSuperLeader(),
            [],
            new Dictionary<string, object?>(),
            new object()
        );

        // Act
        filter.OnActionExecuting(context);

        // Assert
        Assert.Null(context.Result);
    }

}//Cls
