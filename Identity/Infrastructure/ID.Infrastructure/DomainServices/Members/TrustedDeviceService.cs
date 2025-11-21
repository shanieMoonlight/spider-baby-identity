using ID.Application.Features.Users;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using ID.Domain.Repos;
using Microsoft.AspNetCore.Identity;
using MyResults;

namespace ID.Infrastructure.DomainServices.Members;
internal class TrustedDeviceService<TUser>(
    IIdUnitOfWork uow,
    UserManager<TUser> _userMgr,
    IIdentityTrustedDeviceRepo _trustedDeviceRepo)
    where TUser : AppUser
{
    public async Task<GenResult<TrustedDeviceDto>> Handle(
        TUser user,
        DeviceFingerprint deviceFingerprint,
        DeviceName deviceName,
        UserAgent userAgent,
        //TrustedUntil trustedUntil,
        CancellationToken cancellationToken)
    {
        // Build ValueObjects

        TrustedUntil trustedUntil = TrustedUntil.CreateNullable(null);

        var validation = Domain.Entities.AppUsers.Validators.TrustedDeviceValidators.Addition.Validate(user, deviceFingerprint, deviceName, userAgent, trustedUntil);
        if (!validation.Succeeded)
            return validation.Convert<TrustedDeviceDto>();

        // Apply to aggregate
        var device = user.TrustDevice(validation.Value!);
        var added = await _trustedDeviceRepo.AddAsync(device, cancellationToken);

        return GenResult<TrustedDeviceDto>.Success(new TrustedDeviceDto(added));
    }

}
