
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Teams.Cmd.UpdatePositionRange;
public class UpdateTeamPositionRangeCmdValidator : IsLeaderValidator<UpdateTeamPositionRangeCmd>
{
    public UpdateTeamPositionRangeCmdValidator()
    {
        RuleFor(p => p.Dto)
          .NotEmpty()
              .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.MinPosition)
              .NotNull()
                  .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.MaxPosition)
                .NotNull()
                    .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p)
                .Must(p => p.Dto.MinPosition < p.Dto.MaxPosition)
                    .WithMessage("MinPosition must be less than MaxPosition");
        });


    }
}


