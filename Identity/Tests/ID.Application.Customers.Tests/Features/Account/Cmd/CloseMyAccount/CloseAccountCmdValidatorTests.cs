using FluentValidation.TestHelper;
using ID.Application.Customers.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Customers.Features.Account.Cmd.CloseMyAccount;

/// <summary>
/// Tests for non auth-validation
/// </summary>
public class CloseMyAccountCmdValidatorTests
{
    private readonly CloseMyAccountCmdValidator _validator;

    //- - - - - - - - - - - - -//

    public CloseMyAccountCmdValidatorTests()
    {
        _validator = new CloseMyAccountCmdValidator();
    }

    //-------------------------//

    [Fact]
    public void Should_have_error_when_TEAMID_is_empty()
    {
        //Arrange
        CloseMyAccountCmd cmd = new(default);


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.TeamId);

    }


    //-------------------------//


    [Fact]
    public void Implements_ACustomerOnlyValidator()
    {
        // Arrange
        var validator = new CloseMyAccountCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<ACustomerLeaderValidator<CloseMyAccountCmd>>();
    }

}//Cls
