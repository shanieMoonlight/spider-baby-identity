
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomer;
public class GetCustomerQryValidator : AMntcMinimumValidator<GetCustomerQry>
{
    public GetCustomerQryValidator()
    {
        RuleFor(p => p.MemberId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        RuleFor(p => p.TeamId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
    }

}//Cls

