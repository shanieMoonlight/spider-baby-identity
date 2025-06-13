using Shouldly;
using ID.Application.Mediatr.Validation;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorDisable;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorDisable;
public class TwoFactorDisableCmdValidatorTests
{
    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        TwoFactorDisableCmdValidator validator = new();

        // Act & Assert
        Assert.IsType<IsAuthenticatedValidator<TwoFactorDisableCmd>>(validator, exactMatch: false);

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<TwoFactorDisableCmd>>();
    }

    //------------------------------------//


}//Cls
