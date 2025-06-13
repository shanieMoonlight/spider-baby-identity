
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateSelf;
public class UpdateSelfCmdValidator : IsAuthenticatedValidator<UpdateSelfCmd>
{
    public UpdateSelfCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.Id)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        });
    }

}//Cls

