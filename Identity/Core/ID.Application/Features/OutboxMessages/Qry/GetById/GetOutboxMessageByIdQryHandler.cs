using MyResults;
using ID.Application.Features.OutboxMessages;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.OutboxMessages;
using ID.Domain.Abstractions.Services.Outbox;

namespace ID.Application.Features.OutboxMessages.Qry.GetById;
internal class GetOutboxMessageByIdQryHandler(IIdentityOutboxMsgsService repo) : IIdQueryHandler<GetOutboxMessageByIdQry, IdOutboxMessageDto>
{

    public async Task<GenResult<IdOutboxMessageDto>> Handle(GetOutboxMessageByIdQry request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        var mdl = await repo.GetByIdAsync(id);

        if (mdl == null)
            return GenResult<IdOutboxMessageDto>.NotFoundResult(IDMsgs.Error.NotFound<IdOutboxMessage>(id));

        return GenResult<IdOutboxMessageDto>.Success(mdl.ToDto());

    }


}//Cls
