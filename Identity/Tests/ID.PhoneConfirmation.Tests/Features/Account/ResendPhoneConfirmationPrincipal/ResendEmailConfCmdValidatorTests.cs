using ID.Application.Mediatr.Validation;
using ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmationPrincipal;
using Shouldly;

namespace ID.PhoneConfirmation.Tests.Features.Account.ResendPhoneConfirmationPrincipal;
public class ResendEmailConfCmdValidatorTests
{
    private readonly ResendPhoneConfirmationPrincipalCmdValidator _validator;

    public ResendEmailConfCmdValidatorTests()
    {
        _validator = new ResendPhoneConfirmationPrincipalCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {

        // Act & Assert
        _validator.ShouldBeAssignableTo<IsAuthenticatedValidator<ResendPhoneConfirmationPrincipalCmd>>();
    }

    //------------------------------------//


}//Cls
