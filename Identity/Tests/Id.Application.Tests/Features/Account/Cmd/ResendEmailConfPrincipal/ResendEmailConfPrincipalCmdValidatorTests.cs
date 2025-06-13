using Shouldly;
using ID.Application.Mediatr.Validation;
using ID.Application.Features.Account.Cmd.ResendEmailConfirmationPrincipal;

namespace ID.Application.Tests.Features.Account.Cmd.ResendEmailConfPrincipal;
public class ResendEmailConfPrincipalCmdValidatorTests
{
    private readonly ResendEmailConfirmationPrincipalValidator _validator;

    public ResendEmailConfPrincipalCmdValidatorTests() =>
        _validator = new ResendEmailConfirmationPrincipalValidator();

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {

        // Act & Assert
        Assert.IsType<IsAuthenticatedValidator<ResendEmailConfirmationPrincipalCmd>>(_validator, exactMatch: false);
        _validator.ShouldBeAssignableTo<IsAuthenticatedValidator<ResendEmailConfirmationPrincipalCmd>>();
    }

    //------------------------------------//


}//Cls
