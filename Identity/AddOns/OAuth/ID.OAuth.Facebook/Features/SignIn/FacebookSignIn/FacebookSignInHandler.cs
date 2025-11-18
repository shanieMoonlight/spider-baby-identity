using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.JWT;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.OAuth.Facebook.Services.Abs;
using MyResults;

namespace ID.OAuth.Facebook.Features.SignIn.FacebookSignIn;
public class FacebookSignInHandler(
    IFindOrCreateService<AppUser> _findOrCreate,
    IJwtPackageProvider _jwtPackageProvider,
    IFacebookAuthenticationService _verifier,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ITwoFactorMsgService _twoFactorMsgService)
    : IIdCommandHandler<FacebookSignInCmd, JwtPackage>
{

    public async Task<GenResult<JwtPackage>> Handle(FacebookSignInCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Combined verification + profile fetch
        var profileResult = await _verifier.VerifyAndGetProfileAsync(dto.AuthToken, dto.Id, cancellationToken);
        if (!profileResult.Succeeded)
            return profileResult.Convert<JwtPackage>();

        var userProfile = profileResult.Value!; // non-null on success

        var userResult = await _findOrCreate.FindOrCreateUserAsync(userProfile, dto, cancellationToken);
        if (!userResult.Succeeded)
            return userResult.Convert<JwtPackage>();

        AppUser user = userResult.Value!;  //Success is non-null

        var tfEnabled = await _2FactorService.IsTwoFactorEnabledAsync(user);

        return tfEnabled
            ? await SendTwoFactorAndReturnJwtPackageAsync(
                user: user,
                team: user.Team!,
                cancellationToken: cancellationToken)
            : await ReturnStandardJwtPackageAsync(
                user: user,
                team: user.Team!,
                currentDeviceId: dto.DeviceId,
                cancellationToken: cancellationToken);


    }

    //-----------------------------//

    private async Task<GenResult<JwtPackage>> ReturnStandardJwtPackageAsync(
        AppUser user,
        Team team,
        string? currentDeviceId = null,
        CancellationToken cancellationToken = default)
    {
        JwtPackage jwtPackage = await _jwtPackageProvider.CreateJwtPackageAsync(
           user: user,
           team: team,
           currentDeviceId: currentDeviceId,
           cancellationToken: cancellationToken);

        return GenResult<JwtPackage>.Success(jwtPackage);
    }


    //- - - - - - - - - - - - - - -//


    private async Task<GenResult<JwtPackage>> SendTwoFactorAndReturnJwtPackageAsync(
        AppUser user,
        Team team,
        CancellationToken cancellationToken = default)
    {
        var tfResultMsg = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);
        if (!tfResultMsg.Succeeded)
            return GenResult<JwtPackage>.Failure(tfResultMsg.Info);

        MfaResultData mfaResultData = tfResultMsg.Value!; //Success is non-null

        JwtPackage jwtPackage = await _jwtPackageProvider.CreateJwtPackageWithTwoFactorRequiredAsync(
           user: user,
           provider: mfaResultData.TwoFactorProvider,
           extraInfo: mfaResultData.ExtraInfo,
           cancellationToken: cancellationToken);

        return GenResult<JwtPackage>.Success(jwtPackage);
    }



}//Cls

