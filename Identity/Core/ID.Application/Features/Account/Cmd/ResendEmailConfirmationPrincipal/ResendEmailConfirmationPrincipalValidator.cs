using ID.Application.Mediatr.Validation;

namespace ID.Application.Features.Account.Cmd.ResendEmailConfirmationPrincipal;
public class ResendEmailConfirmationPrincipalValidator : IsAuthenticatedValidator<ResendEmailConfirmationPrincipalCmd>
{
    public ResendEmailConfirmationPrincipalValidator()
    { }
}

