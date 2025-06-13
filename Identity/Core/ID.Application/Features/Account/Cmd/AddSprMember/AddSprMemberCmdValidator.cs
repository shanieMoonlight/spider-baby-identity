using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.AddSprMember;
public class AddSprMemberCmdValidator : ASuperMinimumValidator<AddSprMemberCmd>
{

    public AddSprMemberCmdValidator() : base(2)
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

