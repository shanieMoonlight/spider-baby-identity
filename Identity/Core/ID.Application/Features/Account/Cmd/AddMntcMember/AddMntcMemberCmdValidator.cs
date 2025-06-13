
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.AddMntcMember;
public class AddMntcMemberCmdValidator : AMntcMinimumValidator<AddMntcMemberCmd>
{
    public AddMntcMemberCmdValidator() : base(2)
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.Email)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        });
    }

}//Cls

