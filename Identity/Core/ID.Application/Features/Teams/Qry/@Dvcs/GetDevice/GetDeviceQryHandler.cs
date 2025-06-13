using MyResults;
using ID.Application.Features.Teams.Cmd.Dvcs;
using ID.Application.AppAbs.ApplicationServices.Principal;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;
using ID.Domain.Abstractions.Services.Teams.Dvcs;

namespace ID.Application.Features.Teams.Qry.Dvcs.GetDevice;
public class GetDeviceQryHandler(IIdPrincipalInfo userInfo, ITeamDeviceServiceFactory deviceServiceFactory)
     : IIdCommandHandler<GetDeviceQry, DeviceDto>
{
    public async Task<GenResult<DeviceDto>> Handle(GetDeviceQry request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var teamId = dto.TeamId ?? userInfo.TeamId();
        var dvcId = dto.DeviceId;

        var getServiceResult = await deviceServiceFactory.GetServiceAsync(teamId, dto.SubscriptionId);
        if (!getServiceResult.Succeeded)
            return getServiceResult.Convert<DeviceDto>();

        ITeamDeviceService service = getServiceResult.Value!;

        var dvc = await service.GetDeviceAsync(dto.DeviceId);

        if (dvc is null)
            return GenResult<DeviceDto>.NotFoundResult(IDMsgs.Error.NotFound<TeamDevice>(dvcId));

        //return sub with new device attached
        return GenResult<DeviceDto>.Success(dvc.ToDto());
    }


}//Cls

