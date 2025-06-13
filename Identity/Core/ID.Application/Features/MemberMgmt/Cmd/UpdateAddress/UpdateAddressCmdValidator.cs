using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateAddress;
public class UpdateAddressCmdValidator : IsAuthenticatedValidator<UpdateAddressCmd>
{
    public UpdateAddressCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.Line1)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.Line2)
               .NotEmpty()
               .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });
    }

}//Cls

