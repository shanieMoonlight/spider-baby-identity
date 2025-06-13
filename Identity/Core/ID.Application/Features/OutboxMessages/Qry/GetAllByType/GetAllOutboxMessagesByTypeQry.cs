using ID.Application.Features.OutboxMessages;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.OutboxMessages.Qry.GetAllByType;

public record GetAllOutboxMessagesByTypeQry(string? Type) : AIdQuery<IEnumerable<IdOutboxMessageDto>>;


