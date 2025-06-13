
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdatePosition;
public class UpdatePositionCmdValidator : IsMntcMinimumLeaderValidator<UpdatePositionCmd>
{
    public UpdatePositionCmdValidator()
    {

        RuleFor(p => p.UserId)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        RuleFor(p => p.NewPosition)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

    }

}//Cls

