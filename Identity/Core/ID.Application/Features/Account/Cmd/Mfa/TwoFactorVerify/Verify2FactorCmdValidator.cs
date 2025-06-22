
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;

/// <summary>
/// User must already be logged in to verify 2 factor authentication.
/// Hence, IsAuthenticatedValidator
/// </summary>
public class Verify2FactorCmdValidator : AbstractValidator<Verify2FactorCmd>
{
    public Verify2FactorCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto is not null, () =>
        {
            RuleFor(p => p.Dto.Code)
              .NotEmpty()
                      .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.Token)
             .NotEmpty()
                     .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        });

    }
}

