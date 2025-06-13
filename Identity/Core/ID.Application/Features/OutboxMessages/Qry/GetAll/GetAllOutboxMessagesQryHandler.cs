using MyResults;
using ID.Application.Features.OutboxMessages;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Outbox;

namespace ID.Application.Features.OutboxMessages.Qry.GetAll;
internal class GetAllOutboxMessagesQryHandler(IIdentityOutboxMsgsService _repo) : IIdQueryHandler<GetAllOutboxMessagesQry, IEnumerable<IdOutboxMessageDto>>
{

    public async Task<GenResult<IEnumerable<IdOutboxMessageDto>>> Handle(GetAllOutboxMessagesQry request, CancellationToken cancellationToken)
    {
        var mdls = await _repo.GetAllAsync();
        var dtos = mdls.Select(mdl => mdl.ToDto());
        return GenResult<IEnumerable<IdOutboxMessageDto>>.Success(dtos);

    }

}//Cls
