using MyResults;
using ID.Application.Features.Teams;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams.Dvcs;

namespace ID.Application.Features.Teams.Cmd.Dvcs.RemoveDevice;
public class RemoveDeviceFromTeamSubscriptionHandler(ITeamDeviceServiceFactory deviceServiceFactory)
    : IIdCommandHandler<RemoveDeviceFromTeamSubscriptionCmd, SubscriptionDto>
{

    public async Task<GenResult<SubscriptionDto>> Handle(RemoveDeviceFromTeamSubscriptionCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var subscriptionId = dto.SubscriptionId;

        var team = request.PrincipalTeam!;

        var getServiceResult = await deviceServiceFactory.GetServiceAsync(team, dto.SubscriptionId);
        if (!getServiceResult.Succeeded)
            return getServiceResult.Convert<SubscriptionDto>();

        ITeamDeviceService service = getServiceResult.Value!;

        var removeDeviceResult = await service.RemoveDeviceAsync(dto.DeviceId);


        return removeDeviceResult.Convert((x) => service.Subscription.ToDto());
    }


}//Cls

