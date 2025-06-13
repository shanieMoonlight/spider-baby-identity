using ID.Application.Features.Account.Qry.GetProviders;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Qry.GetProviders;

public class GetProvidersQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidatorr()
    {
        // Arrange
        var validator = new GetProvidersQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<GetProvidersQry>>();
    }

    //------------------------------------//

}//Cls