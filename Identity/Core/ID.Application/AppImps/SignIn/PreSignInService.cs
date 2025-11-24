using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.EventBuses;
using ID.Application.AppAbs.FromApp;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Features.Account.Cmd.Login;
using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Claims.AuthMethods;
using ID.Domain.Entities.Teams;
using Microsoft.Extensions.Logging;

namespace ID.Application.AppImps.SignIn;

internal class PreSignInService<TUser>(
    IIdUserMgmtService<TUser> _userMgr,
    IFindUserService<TUser> _findUserService,
    IEmailConfirmationBus _emailConfirmationBus,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ITwoFactorMsgService _twoFactorMsgService,
    IIsFromMobileApp _fromAppService,
    ITrustedDeviceService<TUser> _trustedDeviceService,
    ILogger<PreSignInService<TUser>> _logger
) : IPreSignInService<TUser>
    where TUser : AppUser
{
    public async Task<MyIdSignInResult> Authenticate(
        LoginDto dto,
        CancellationToken cancellationToken
    )
    {
        //Check if user exists
        var user = await _findUserService.FindUserWithTeamDetailsAsync(
            dto.Email,
            dto.Username,
            dto.UserId
        );
        
        if (user == null)
            return MyIdSignInResult.NotFoundResult();

        if (!await _userMgr.IsEmailConfirmedAsync(user))
        {
            await _emailConfirmationBus.GenerateTokenAndPublishEventAsync(
                user,
                user.Team!,
                cancellationToken
            );
            return MyIdSignInResult.EmailConfirmedRequiredResult(user.Email ?? "no-email");
        }

        bool success = await _userMgr.CheckPasswordAsync(user, dto.Password ?? "");
        if (!success)
            return MyIdSignInResult.UnauthorizedResult();

        var trustedBypassed = await UseTrustedDeviceIfValid(dto.DeviceId, user, cancellationToken);
        if (trustedBypassed)
            return MyIdSignInResult.Success(user, user.Team!, [AuthMethodRef.mfa]); //Trusted Device is MFA 

        //Package all user info  and send it back to client.
        var tfEnabled = await _2FactorService.IsTwoFactorEnabledAsync(user);
        if (tfEnabled && !_fromAppService.IsFromApp)
            return await SendTwoFactor(user, user.Team!);

        return MyIdSignInResult.Success(user, user.Team!, []);
    }
    
    //-----------------------------//

    private async Task<bool> UseTrustedDeviceIfValid(string? deviceId, AppUser user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            return false;

        try
        {
            var trustedDevice = await _trustedDeviceService.GetByFingerprintAsync((TUser)user, deviceId, cancellationToken);
            if (trustedDevice is not null && !trustedDevice.IsExpired())
            {
                await _trustedDeviceService.UpdateLastUsedAsync((TUser)user, trustedDevice, cancellationToken);
                _logger.LogInformation("Bypassing 2FA for user {UserId} using trusted device (fingerprint hash).", user.Id);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Trusted device {DeviceId} check failed for user {UserId}; continuing with MFA.", deviceId, user.Id);
        }

        return false;
    }

    //-----------------------------//

    private async Task<MyIdSignInResult> SendTwoFactor(AppUser user, Team team)
    {
        var tfResultMsg = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);
        return !tfResultMsg.Succeeded
            ? MyIdSignInResult.Failure(tfResultMsg.Info)
            : MyIdSignInResult.TwoFactorRequiredResult(tfResultMsg.Value, user, team);
    }
} //Cls
