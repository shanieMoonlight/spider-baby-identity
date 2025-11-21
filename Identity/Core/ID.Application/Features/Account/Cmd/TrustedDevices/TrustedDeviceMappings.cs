using ID.Domain.Entities.TrustedDevices;

namespace ID.Application.Features.Account.Cmd.TrustedDevices;

public static class TrustedDeviceMappings
{

    public static TrustedDeviceDto ToDto(this TrustedDevice mdl) =>
        new(mdl);


}//Cls


