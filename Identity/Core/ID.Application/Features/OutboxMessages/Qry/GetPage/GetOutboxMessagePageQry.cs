using ID.Application.Features.OutboxMessages;
using ID.Application.Mediatr.Cqrslmps.Queries;
using Pagination;

namespace ID.Application.Features.OutboxMessages.Qry.GetPage;

public record GetOutboxMessagePageQry(PagedRequest? PagedRequest) : AIdQuery<PagedResponse<IdOutboxMessageDto>>;


