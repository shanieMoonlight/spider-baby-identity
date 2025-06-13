
using FluentValidation;
using ID.Application.Customers.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Customers.Features.MemberMgmt.Cmd.DeleteCustomerMember;
public class DeleteCustomerMemberCmdValidator : ACustomerOnlyValidator<DeleteCustomerMemberCmd>
{
    public DeleteCustomerMemberCmdValidator()
    {
        RuleFor(p => p.UserId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }
}//Cls


