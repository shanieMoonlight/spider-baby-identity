using MyResults;
using ID.Application.Features.Teams;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams.Subs;

namespace ID.Application.Features.Teams.Cmd.Subs.RecordSubscriptionPayment;
public class RecordSubscriptionPaymentCmdHandler(ITeamSubscriptionServiceFactory subsServiceFactory)
     : IIdCommandHandler<RecordSubscriptionPaymentCmd, SubscriptionDto>
{

    public async Task<GenResult<SubscriptionDto>> Handle(RecordSubscriptionPaymentCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var getServiceResult = await subsServiceFactory.GetServiceAsync(dto.TeamId);
        if (!getServiceResult.Succeeded)
            return getServiceResult.Convert<SubscriptionDto>();

        ITeamSubscriptionService service = getServiceResult.Value!;

        var recordResult = await service.RecordPaymentAsync(dto.SubscriptionId);
        return recordResult.Convert(s => s?.ToDto());
    }

}//Cls

