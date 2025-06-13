using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Application.Features.OutboxMessages.Qry.GetById;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.OutboxMessages.Qry.GetAllByType;
public class GetAllOutboxMessagesByTypeQryValidator
    : AMntcMinimumValidator<GetAllOutboxMessagesByTypeQry>
{
    public GetAllOutboxMessagesByTypeQryValidator()
    {
        RuleFor(p => p.Type)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

    }

}
