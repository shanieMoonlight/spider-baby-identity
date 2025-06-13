
using FluentValidation;
using ID.Application.Customers.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Customers.Features.Account.Cmd.AddCustomerMember;
public class AddCustomerMemberCmdValidator : ACustomerMinimumValidator<AddCustomerMemberCmd>
{
    public AddCustomerMemberCmdValidator()
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

