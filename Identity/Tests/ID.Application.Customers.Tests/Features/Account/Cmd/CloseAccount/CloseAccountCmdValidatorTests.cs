using ID.Application.Customers.Features.Account.Cmd.CloseAccount;
using ID.Application.Customers.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.CloseAccount;

/// <summary>
/// Tests for non auth-validation
/// </summary>
public class CloseAccountCmdValidatorTests
{
    private readonly CloseMyAccountCmdValidator _validator;

    //- - - - - - - - - - - - - - - - - - //

    public CloseAccountCmdValidatorTests()
    {
        _validator = new CloseMyAccountCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Implements_ACustomerOnlyValidator()
    {
        // Arrange
        var validator = new CloseMyAccountCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<ACustomerLeaderValidator<CloseMyAccountCmd>>();
    }

    //------------------------------------//

}//Cls
