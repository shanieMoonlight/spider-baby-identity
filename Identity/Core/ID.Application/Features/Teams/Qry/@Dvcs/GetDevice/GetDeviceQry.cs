using ID.Application.Features.Teams.Cmd.Dvcs;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Teams.Qry.Dvcs.GetDevice;
public record GetDeviceQry(GetDeviceDto Dto) : AIdCommand<DeviceDto>;
