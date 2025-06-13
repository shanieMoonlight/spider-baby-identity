using ID.Application.Features.OutboxMessages;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.OutboxMessages.Qry.GetAll;
public record GetAllOutboxMessagesQry(Guid? Id = null) : AIdQuery<IEnumerable<IdOutboxMessageDto>>;
