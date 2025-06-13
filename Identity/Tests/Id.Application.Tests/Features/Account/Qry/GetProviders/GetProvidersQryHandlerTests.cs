using ID.Application.Features.Account.Qry.GetProviders;
using ID.Application.Utility.Enums;
using ID.Domain.Models;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Qry.GetProviders;

public class GetProvidersQryHandlerTests
{

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnProviders()
    {
        // Arrange
        var handler = new GetProvidersQryHandler();
        var request = new GetProvidersQry();
        var cancellationToken = CancellationToken.None;


        // Act
        var result = await handler.Handle(request, cancellationToken);


        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeOfType<string[]>();
        result.Value.ShouldBe(MyEnums.GetDescriptions<TwoFactorProvider>());
    }

    //------------------------------------//

}//Cls