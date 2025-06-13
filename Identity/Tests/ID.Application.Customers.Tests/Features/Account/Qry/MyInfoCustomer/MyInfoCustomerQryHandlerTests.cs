using ID.Application.Customers.Features.Account.Qry.MyInfoCustomer;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Qry.MyInfoCustomer;

public class MyInfoCustomerQryHandlerTests
{

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserIsNotNull()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamId: Guid.NewGuid());
        var request = new MyInfoCustomerQry { PrincipalUser = user };
        var handler = new MyInfoCustomerQryHandler();
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