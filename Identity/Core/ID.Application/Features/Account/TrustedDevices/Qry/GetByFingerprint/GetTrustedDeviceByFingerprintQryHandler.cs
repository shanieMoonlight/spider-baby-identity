using ID.Application.Features.Account.TrustedDevices;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.TrustedDevices.Qry.GetByFingerprint;
internal class GetTrustedDeviceByFingerprintQryHandler() : IIdQueryHandler<GetTrustedDeviceByFingerprintQry, TrustedDeviceDto>
{

    public Task<GenResult<TrustedDeviceDto>> Handle(GetTrustedDeviceByFingerprintQry request, CancellationToken cancellationToken)
    {
        var fingerprint = request.DeviceFingerprint;
        var user = request.PrincipalUser;

        var mdl = user.FindTrustedDevice(fingerprint);
        if (mdl == null)
            return Task.FromResult(GenResult<TrustedDeviceDto>.NotFoundResult(IDMsgs.Error.NotFound<TrustedDevice>(fingerprint)));

        return Task.FromResult(GenResult<TrustedDeviceDto>.Success(mdl.ToDto()));

    }


}//Cls
