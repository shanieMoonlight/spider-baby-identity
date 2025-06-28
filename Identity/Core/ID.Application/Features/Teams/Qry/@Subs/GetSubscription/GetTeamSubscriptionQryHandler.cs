using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Teams.Qry.@Subs.GetSubscription;
public class GetTeamSubscriptionQryHandler(ITeamSubscriptionServiceFactory subsServiceFactory) : IIdCommandHandler<GetTeamSubscriptionQry, SubscriptionDto>
{

    public async Task<GenResult<SubscriptionDto>> Handle(GetTeamSubscriptionQry request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var teamId = dto.TeamId ?? request.PrincipalTeamId;
        var subId = dto.SubscriptionId;

        var getServiceResult = await subsServiceFactory.GetServiceAsync(dto.TeamId);
        if (!getServiceResult.Succeeded)
            return getServiceResult.Convert<SubscriptionDto>();

        ITeamSubscriptionService service = getServiceResult.Value!;

        var sub = await service.GetSubscriptionAsync(subId);
        if (sub is null)
            return GenResult<SubscriptionDto>.NotFoundResult(IDMsgs.Error.NotFound<TeamDevice>(subId));

        //return sub with new device attached
        return GenResult<SubscriptionDto>.Success(sub.ToDto());

    }

}//Cls

