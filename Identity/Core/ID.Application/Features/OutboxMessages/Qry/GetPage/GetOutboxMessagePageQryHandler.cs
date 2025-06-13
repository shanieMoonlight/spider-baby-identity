using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Outbox;
using MyResults;
using Pagination;

namespace ID.Application.Features.OutboxMessages.Qry.GetPage;
internal class GetOutboxMessagePageQryHandler(IIdentityOutboxMsgsService _repo) : IIdQueryHandler<GetOutboxMessagePageQry, PagedResponse<IdOutboxMessageDto>>
{
    public async Task<GenResult<PagedResponse<IdOutboxMessageDto>>> Handle(GetOutboxMessagePageQry request, CancellationToken cancellationToken)
    {

        var pgRequest = request.PagedRequest ?? PagedRequest.Empty();

        var page = (await _repo.GetPageAsync(pgRequest.PageNumber, pgRequest.PageSize, pgRequest.SortList, pgRequest.FilterList))
                   .Transform((d) => d.ToDto());

        var response = new PagedResponse<IdOutboxMessageDto>(page, pgRequest);

        return GenResult<PagedResponse<IdOutboxMessageDto>>.Success(response);

    }

}//Cls

