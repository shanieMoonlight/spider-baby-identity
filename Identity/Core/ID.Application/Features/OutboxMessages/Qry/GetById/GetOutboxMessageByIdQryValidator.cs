using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.OutboxMessages.Qry.GetById;
public class GetOutboxMessageByIdQryValidator
    : AMntcMinimumValidator<GetOutboxMessageByIdQry>
{
    public GetOutboxMessageByIdQryValidator()
    {
        RuleFor(p => p.Id)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

    }

}
