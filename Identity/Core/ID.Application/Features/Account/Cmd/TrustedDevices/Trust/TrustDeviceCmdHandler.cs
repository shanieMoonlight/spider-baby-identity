//using ID.Domain.Entities.TrustedDevices.ValueObjects;
//using ID.Domain.Entities.AppUsers.Validators;
//using ID.Domain.Repos;
//using Microsoft.Extensions.Logging;
//using MyResults;
//using ID.Application.Features.Users;
//using ID.Application.Mediatr.CqrsAbs;

//namespace ID.Application.Features.Account.Cmd.TrustedDevices.Trust;

//public class TrustDeviceCmdHandler(IIdentityTrustedDeviceRepo trustedDeviceRepo, ILogger<TrustDeviceCmdHandler> logger) : IIdCommandHandler<TrustDeviceCmd, TrustedDeviceDto>
//{
//    public async Task<GenResult<TrustedDeviceDto>> Handle(TrustDeviceCmd request, CancellationToken cancellationToken)
//    {
//        var user = request.PrincipalUser;

//        // Build ValueObjects
//        var fingerprint = DeviceFingerprint.Create(request.Dto.Fingerprint);
//        var name = DeviceName.Create(request.Dto.Name);
//        var userAgent = UserAgent.CreateNullable(request.Dto.UserAgent);

//        TrustedUntil trustedUntil = request.Dto.TrustDays.HasValue
//            ? TrustedUntil.CreateNullable(DateTime.UtcNow.AddDays(request.Dto.TrustDays.Value).ToUniversalTime())
//            : TrustedUntil.CreateNullable(null);

//        var validation = TrustedDeviceValidators.Addition.Validate(user, fingerprint, name, userAgent, trustedUntil);
//        if (!validation.Succeeded)
//            return validation.Convert<TrustedDeviceDto>();

//        // Apply to aggregate
//        var device = user.TrustDevice(validation.Value!);

//        try
//        {
//            var added = await trustedDeviceRepo.AddAsync(device, cancellationToken);
//            return GenResult<TrustedDeviceDto>.Success(new TrustedDeviceDto(added));
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "Error adding trusted device for user {UserId}", user.Id);
//            return GenResult<TrustedDeviceDto>.Failure(ex, "Failed to persist trusted device");
//        }
//    }
//}
