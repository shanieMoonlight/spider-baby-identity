using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppAbs.TrustedDevices;
using ID.Application.JWT;
using ID.Application.MFA;
using ID.Domain.Claims.AuthMethods;
using ID.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;
public class Verify2FactorHandler(
    IJwtPackageProvider _jwtPackageProvider,
    IFindUserService<AppUser> _findUserService,
    ITwofactorUserIdCacheService _twofactorUserIdCache,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    IDeviceTrustService<AppUser> _deviceTrustService,
    ILogger<Verify2FactorHandler> _logger)
    : IIdCommandHandler<Verify2FactorCmd, JwtPackage>
{

    public async Task<GenResult<JwtPackage>> Handle(Verify2FactorCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var userId = _twofactorUserIdCache.GetUserId(dto.Token);
        var user = await _findUserService.FindUserWithTeamDetailsAsync(userId: userId);
        var team = user?.Team; 


        if (user is null || team is null)
            return GenResult<JwtPackage>.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE); //Just blame it on the token no more info should be revealed


        bool validVerification = await _2FactorService.VerifyTwoFactorTokenAsync(team, user, dto.Code);
        if (!validVerification)
            return GenResult<JwtPackage>.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE);

        // If the client asked to trust this device, persist it now
        if (dto.TrustDevice && !string.IsNullOrWhiteSpace(dto.DeviceFingerprint))
        {
            try
            {
                var addResult = await _deviceTrustService.TrustAsync(
                    user: user,
                    deviceFingerprint: dto.DeviceFingerprint!,
                    deviceName: "Trusted via MFA",
                    cancellationToken: cancellationToken);

                if (!addResult.Succeeded)
                    _logger.LogWarning("Failed to create trusted device for user {UserId}: {Error}", user.Id, addResult.Info);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception while creating trusted device for user {UserId}; continuing.", user.Id);
            }
        }

        // Package all user info in JWT and send it back to client.
        JwtPackage jwtPackage = await _jwtPackageProvider.CreateJwtPackageAsync(
            user: user,
            team: user.Team!,
            [AuthMethodRef.mfa],
            currentDeviceId: dto.DeviceId,
            cancellationToken: cancellationToken);

        return GenResult<JwtPackage>.Success(jwtPackage);
    }


}//Cls
