using MyResults;
using ID.Application.Features.OutboxMessages;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Outbox;

namespace ID.Application.Features.OutboxMessages.Qry.GetAllByType;
internal class GetAllOutboxMessagesFilteredQryHandler(IIdentityOutboxMsgsService _repo)
    : IIdQueryHandler<GetAllOutboxMessagesByTypeQry, IEnumerable<IdOutboxMessageDto>>
{

    public async Task<GenResult<IEnumerable<IdOutboxMessageDto>>> Handle(GetAllOutboxMessagesByTypeQry request, CancellationToken cancellationToken)
    {
        var mdls = await _repo.GetAllByTypeAsync(request.Type);
        var dtos = mdls.Select(mdl => mdl.ToDto());
        return GenResult<IEnumerable<IdOutboxMessageDto>>.Success(dtos);
    }


}//Cls

