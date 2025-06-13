
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.SubscriptionPlans.Cmd.Delete;
public class DeleteSubscriptionPlanCmdValidator : AMntcMinimumValidator<DeleteSubscriptionPlanCmd>
{
    public DeleteSubscriptionPlanCmdValidator()
    {
        RuleFor(p => p.Id)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

    }
}//Cls


