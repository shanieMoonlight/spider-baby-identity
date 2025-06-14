using ClArch.ValueObjects;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Customers.Abstractions;
using ID.Application.JWT;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.OAuth.Google.Auth.Abs;
using ID.OAuth.Google.Data;
using MyResults;

namespace ID.OAuth.Google.Features.SignIn.GoogleSignIn;
public class GoogleSignInHandler(
    IFindUserService<AppUser> _findUserService,
    IJwtPackageProvider _jwtPackageProvider,
    IGoogleTokenVerifier _verifier,
    IIdCustomerRegistrationService _signupService,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ITwoFactorMsgService _twoFactorMsgService)
    : IIdCommandHandler<GoogleSignInCmd, JwtPackage>
{

    public async Task<GenResult<JwtPackage>> Handle(GoogleSignInCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;


        var verifyResult = await _verifier.VerifyTokenAsync(dto.IdToken, cancellationToken);
        if (!verifyResult.Succeeded)
            return verifyResult.Convert<JwtPackage>();


        var payload = verifyResult.Value!; //Success is non-null


        var userResult = await FindOrCreateUserAsync(payload, dto, cancellationToken);
        if (!userResult.Succeeded)
            return userResult.Convert<JwtPackage>();

        AppUser user = userResult.Value!;  //Success is non-null


        var tfEnabled = await _2FactorService.IsTwoFactorEnabledAsync(user);

        return tfEnabled
            ? await SendTwoFactoAndReturnJwtPackageAsync(
                user: user,
                team: user.Team!,
                currentDeviceId: dto.DeviceId,
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
           twoFactorVerified: false,
           currentDeviceId: currentDeviceId,
           cancellationToken: cancellationToken);

        return GenResult<JwtPackage>.Success(jwtPackage);
    }


    //- - - - - - - - - - - - - - -//


    private async Task<GenResult<JwtPackage>> SendTwoFactoAndReturnJwtPackageAsync(
        AppUser user,
        Team team,
        string? currentDeviceId = null,
        CancellationToken cancellationToken = default)
    {
        var tfResultMsg = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);
        if (!tfResultMsg.Succeeded)
            return GenResult<JwtPackage>.Failure(tfResultMsg.Info);

        MfaResultData mfaResultData = tfResultMsg.Value!; //Success is non-null

        JwtPackage jwtPackage = await _jwtPackageProvider.CreateJwtPackageWithTwoFactorRequiredAsync(
           user: user,
           team: team,
           provider: mfaResultData.TwoFactorProvider,
           extraInfo: mfaResultData.ExtraInfo,
           currentDeviceId: currentDeviceId,
           cancellationToken: cancellationToken);

        return GenResult<JwtPackage>.Success(jwtPackage);
    }


    //- - - - - - - - - - - - - - -//


    private async Task<GenResult<AppUser>> FindOrCreateUserAsync(
        GoogleVerifiedPayload payload,
        GoogleSignInDto dto,
        CancellationToken cancellationToken)
    {
        var user = await _findUserService.FindUserWithTeamDetailsAsync(email: payload.Email);

        if (user != null)
            return GenResult<AppUser>.Success(user);

        OAuthInfo oAuth = OAuthInfo.Create(
            OAuthProvider.Google,
            IssuerNullable.Create(payload.Issuer),
            ImgUrlNullable.Create(payload.Picture),
            EmailVerifiedNullable.Create(payload.EmailVerified));



        return await _signupService.RegisterOAuthAsync(
                    EmailAddress.Create(payload.Email),
                    UsernameNullable.Create(payload.Email),
                    PhoneNullable.Create(null),
                    FirstNameNullable.Create(payload.GivenName),
                    LastNameNullable.Create(payload.FamilyName),
                    TeamPositionNullable.Create(),
                    oAuth,
                    dto.SubscriptionId,
                    cancellationToken);
    }

    //- - - - - - - - - - - - - - -//

    private async Task<MyIdSignInResult> SendTwoFactor(AppUser user, Team team)
    {
        var tfResultMsg = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);
        return !tfResultMsg.Succeeded
            ? MyIdSignInResult.Failure(tfResultMsg.Info)
            : MyIdSignInResult.TwoFactorRequiredResult(tfResultMsg.Value, user, team);
    }



}//Cls

