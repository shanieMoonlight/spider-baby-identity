
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.GlobalSettings.Utility;

namespace ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmationPrincipal;
public class ResendPhoneConfirmationPrincipalCmdValidator : IsAuthenticatedValidator<ResendPhoneConfirmationPrincipalCmd>
{
    public ResendPhoneConfirmationPrincipalCmdValidator()
    {

    }
}

