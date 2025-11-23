using ID.Application.Features.Account.TrustedDevices;

namespace ID.Application.Features.Account.TrustedDevices.Qry.GetById;
internal class GetTrustedDeviceByIdQryHandler() : IIdQueryHandler<GetTrustedDeviceByIdQry, TrustedDeviceDto>
{

    public Task<GenResult<TrustedDeviceDto>> Handle(GetTrustedDeviceByIdQry request, CancellationToken cancellationToken)
    {
        var deviceId = request.Id;
        var user = request.PrincipalUser;

        var mdl = user.FindTrustedDevice(deviceId);
        if (mdl is null)
            return Task.FromResult(GenResult<TrustedDeviceDto>.NotFoundResult(IDMsgs.Error.NotFound<TrustedDevice>(deviceId)));

        return Task.FromResult(GenResult<TrustedDeviceDto>.Success(mdl.ToDto()));

    }


}//Cls
