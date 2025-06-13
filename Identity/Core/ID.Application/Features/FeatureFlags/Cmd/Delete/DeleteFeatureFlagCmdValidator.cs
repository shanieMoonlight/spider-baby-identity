using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.FeatureFlags.Cmd.Delete;
public class DeleteFeatureFlagCmdValidator : AMntcMinimumValidator<DeleteFeatureFlagCmd>
{
    public DeleteFeatureFlagCmdValidator()
    {
        RuleFor(p => p.Id)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

    }
}//Cls
