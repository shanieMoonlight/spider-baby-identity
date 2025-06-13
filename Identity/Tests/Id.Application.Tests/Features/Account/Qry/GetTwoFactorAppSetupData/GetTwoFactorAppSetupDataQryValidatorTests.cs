using ID.Application.Features.Account.Qry.GetTwoFactorAppSetupData;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Qry.GetTwoFactorAppSetupData;

public class GetTwoFactorAppSetupDataQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidatorr()
    {
        // Arrange
        var validator = new GetTwoFactorAppSetupDataQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<GetTwoFactorAppSetupDataQry>>();
    }

    //------------------------------------//

}//Cls