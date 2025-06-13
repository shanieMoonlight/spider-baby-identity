using ClArch.ValueObjects;
using MyResults;
using ID.Application.Features.Teams;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams.Dvcs;

namespace ID.Application.Features.Teams.Cmd.Dvcs.UpdateDevice;
public class UpdateDeviceHandler(ITeamDeviceServiceFactory deviceServiceFactory)
     : IIdCommandHandler<UpdateDeviceCmd, SubscriptionDto>
{
    public async Task<GenResult<SubscriptionDto>> Handle(UpdateDeviceCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var team = request.PrincipalTeam; //Can only add device to your own team

        var getServiceResult = await deviceServiceFactory.GetServiceAsync(team, dto.SubscriptionId);
        if (!getServiceResult.Succeeded)
            return getServiceResult.Convert<SubscriptionDto>();

        ITeamDeviceService service = getServiceResult.Value!;


        var addDeviceResult = await service.UpdateDeviceAsync(
            dto.Id,
            Name.Create(dto.Name),
            DescriptionNullable.Create(dto.Description));


        return addDeviceResult.Convert((x) => service.Subscription.ToDto());
    }

}//Cls

