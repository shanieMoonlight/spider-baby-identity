//using ID.Domain.Entities.AppUsers.Validators;
//using ID.Domain.Repos;
//using Microsoft.Extensions.Logging;
//using MyResults;
//using ID.Application.Mediatr.CqrsAbs;

//namespace ID.Application.Features.Account.Cmd.TrustedDevices.Revoke;

//public class RevokeTrustedDeviceCmdHandler(IIdentityTrustedDeviceRepo trustedDeviceRepo, ILogger<RevokeTrustedDeviceCmdHandler> logger) : IIdCommandHandler<RevokeTrustedDeviceCmd>
//{
//    public async Task<BasicResult> Handle(RevokeTrustedDeviceCmd request, CancellationToken cancellationToken)
//    {
//        var user = request.PrincipalUser;

//        var device = await trustedDeviceRepo.FirstOrDefaultByIdAsync(request.Dto.DeviceId);
//        if (device is null)
//            return BasicResult.NotFoundResult("Trusted device not found");

//        var validation = TrustedDeviceValidators.Revocation.Validate(user, device);
//        if (!validation.Succeeded)
//            return BasicResult.BadRequestResult(validation.Info);

//        var revoked = user.RevokeTrustedDevice(validation.Value!);
//        if (!revoked)
//            return BasicResult.Failure("Failed to revoke trusted device");

//        try
//        {
//            await trustedDeviceRepo.UpdateAsync(device);
//            return BasicResult.Success();
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "Error revoking trusted device {DeviceId} for user {UserId}", device.Id, user.Id);
//            return BasicResult.Failure(ex, "Failed to persist trusted device revocation");
//        }
//    }
//}
