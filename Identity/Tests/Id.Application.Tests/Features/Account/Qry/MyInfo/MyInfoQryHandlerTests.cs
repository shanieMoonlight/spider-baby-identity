using ID.Application.Features.Account.Qry.MyInfo;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Qry.MyInfo;

public class MyInfoQryHandlerTests
{

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserIsNotNull()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamId: Guid.NewGuid());
        var request = new MyInfoQry { PrincipalUser = user };
        var handler = new MyInfoQryHandler();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldNotBeNull();
        result.Value.UserName.ShouldBe(user.UserName);
    }

    //------------------------------------//

}//Cls