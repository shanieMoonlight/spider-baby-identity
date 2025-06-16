using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdatePosition;
public class UpdatePositionCmdValidator : IsMntcMinimumLeaderValidator<UpdatePositionCmd>
{
    public UpdatePositionCmdValidator()
    {

        RuleFor(p => p.Dto)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


        When(p => p.Dto != null, () =>
        {

            RuleFor(p => p.Dto.UserId)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });

    }

}//Cls

