using ID.Application.Features.OutboxMessages;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.OutboxMessages.Qry.GetById;
public record GetOutboxMessageByIdQry(Guid? Id) : AIdQuery<IdOutboxMessageDto>;
