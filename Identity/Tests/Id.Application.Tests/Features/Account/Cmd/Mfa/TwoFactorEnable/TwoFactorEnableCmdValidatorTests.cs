using Shouldly;
using ID.Application.Mediatr.Validation;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorEnable;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorEnable;
public class TwoFactorEnableCmdValidatorTests
{
    private readonly TwoFactorEnableCmdValidator _validator;

    public TwoFactorEnableCmdValidatorTests()
    {
        _validator = new TwoFactorEnableCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        TwoFactorEnableCmdValidator validator = new();

        // Act & Assert
        Assert.IsType<IsAuthenticatedValidator<TwoFactorEnableCmd>>(validator, exactMatch: false);

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<TwoFactorEnableCmd>>();
    }

    //------------------------------------//


}//Cls
