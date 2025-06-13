using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.FeatureFlags.Cmd.Create;
public class CreateFeatureFlagCmdValidator : AMntcMinimumValidator<CreateFeatureFlagCmd>
{
    public CreateFeatureFlagCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }
}//Cls
