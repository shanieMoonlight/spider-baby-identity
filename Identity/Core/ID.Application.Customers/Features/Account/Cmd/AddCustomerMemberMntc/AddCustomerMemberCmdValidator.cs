
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Customers.Features.Account.Cmd.AddCustomerMemberMntc;
public class AddCustomerMemberCmd_MntcValidator : AMntcMinimumValidator<AddCustomerMemberCmd_Mntc>
{
    public AddCustomerMemberCmd_MntcValidator()
    {
        RuleFor(p => p.Dto)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.Email)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));


            RuleFor(p => p.Dto.TeamId)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        });
    }

}//Cls

