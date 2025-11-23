using ID.Domain.Entities.TrustedDevices;

namespace ID.Application.Features.Account.TrustedDevices;

public static class TrustedDeviceMappings
{

    public static TrustedDeviceDto ToDto(this TrustedDevice mdl) =>
        new(mdl);


}//Cls


