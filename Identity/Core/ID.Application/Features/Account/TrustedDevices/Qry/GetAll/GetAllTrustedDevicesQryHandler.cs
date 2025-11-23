using ID.Application.Features.Account.TrustedDevices;
using ID.Application.Mediatr.CqrsAbs;
using MyResults;

namespace ID.Application.Features.Account.TrustedDevices.Qry.GetAll;
internal class GetAllTrustedDevicesQryHandler() : IIdQueryHandler<GetAllTrustedDevicesQry, IEnumerable<TrustedDeviceDto>>
{

    public Task<GenResult<IEnumerable<TrustedDeviceDto>>> Handle(GetAllTrustedDevicesQry request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser;
        var dtos = user.TrustedDevices.Select(mdl => mdl.ToDto());
        return   Task.FromResult(GenResult<IEnumerable<TrustedDeviceDto>>.Success(dtos));

    }

}//Cls
