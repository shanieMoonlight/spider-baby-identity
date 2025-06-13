using ClArch.ValueObjects;
using MyResults;
using ID.Application.Features.Teams;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.Devices;
using ID.Domain.Abstractions.Services.Teams.Dvcs;

namespace ID.Application.Features.Teams.Cmd.Dvcs.AddDevice;
public class AddDeviceToTeamHandler(ITeamDeviceServiceFactory deviceServiceFactory)
     : IIdCommandHandler<AddDeviceToTeamCmd, SubscriptionDto>
{
    public async Task<GenResult<SubscriptionDto>> Handle(AddDeviceToTeamCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var team = request.PrincipalTeam; //Can only add device to your own team

        var getServiceResult = await deviceServiceFactory.GetServiceAsync(team, dto.SubscriptionId);
        if (!getServiceResult.Succeeded)
            return getServiceResult.Convert<SubscriptionDto>();

        ITeamDeviceService service = getServiceResult.Value!;

        var addDeviceResult = await service.AddDeviceAsync(
                         Name.Create(dto.Name),
                         DescriptionNullable.Create(dto.Description),
                         UniqueId.Create(dto.UniqueId));


        return addDeviceResult.Convert((x) => service.Subscription.ToDto());

    }


}//Cls

