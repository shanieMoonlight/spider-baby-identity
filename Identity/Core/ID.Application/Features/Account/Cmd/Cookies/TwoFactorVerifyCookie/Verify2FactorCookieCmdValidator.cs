
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Cookies.TwoFactorVerifyCookie;

/// <summary>
/// User must already be logged in to verify 2 factor authentication.
/// Hence, IsAuthenticatedValidator
/// </summary>
public class Verify2FactorCookieCmdValidator : IsAuthenticatedValidator<Verify2FactorCookieCmd>
{
    public Verify2FactorCookieCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto is not null, () =>
        {
            RuleFor(p => p.Dto.Token)
              .NotEmpty()
                      .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        });

    }
}

