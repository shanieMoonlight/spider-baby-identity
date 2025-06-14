using ClArch.ValueObjects;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Customers.Abstractions;
//using ID.Application.Features.Account.Cmd.Cookies.SignIn;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.Domain.Utility.Messages;
using ID.OAuth.Google.Auth.Abs;
using ID.OAuth.Google.Data;
using MyResults;

namespace ID.OAuth.Google.Features.SignIn.GoogleCookieSignIn;
public class GoogleCookieSignInCmdHandler(
    IFindUserService<AppUser> _findUserService,
    ICookieSignInService<AppUser> _cookieSignInService,
    IGoogleTokenVerifier _verifier,
    IIdCustomerRegistrationService _signupService,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ITwoFactorMsgService _twoFactorMsgService)
    : IIdCommandHandler<GoogleCookieSignInCmd, CookieSignInResultData>
{

    public async Task<GenResult<CookieSignInResultData>> Handle(GoogleCookieSignInCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;


        var verifyResult = await _verifier.VerifyTokenAsync(dto.IdToken, cancellationToken);
        if (!verifyResult.Succeeded)
            return verifyResult.Convert<CookieSignInResultData>();


        var payload = verifyResult.Value!; //Success is non-null


        var userResult = await FindOrCreateUserAsync(payload, dto, cancellationToken);
        if (!userResult.Succeeded)
            return userResult.Convert<CookieSignInResultData>();

        AppUser user = userResult.Value!;  //Success is non-null


        var tfEnabled = await _2FactorService.IsTwoFactorEnabledAsync(user);

        if (tfEnabled)
        {
            var twoFactorResult = await SendTwoFactoAndAttachCookieAsync(
                   user: user,
                   team: user.Team!,
                   dto.RememberMe,
                   currentDeviceId: dto.DeviceId);

            if (!twoFactorResult.Succeeded)
                return twoFactorResult.Convert<CookieSignInResultData>();

            var mfaResultData = twoFactorResult.Value!; //Success is non-null
            var data = CookieSignInResultData.CreateWithTwoFactoRequired(
                provider: mfaResultData.TwoFactorProvider,
                message: IDMsgs.Error.Authorization.TWO_FACTOR_REQUIRED(mfaResultData.TwoFactorProvider));

            return GenResult<CookieSignInResultData>.PreconditionRequiredResult(data); 
        }
        else
        {
            await AttachCookieAsync(
                 user: user,
                 team: user.Team!,
                 dto.RememberMe,
                 currentDeviceId: dto.DeviceId);
        }

        return GenResult<CookieSignInResultData>.Success(CookieSignInResultData.Success());


    }

    //-----------------------------//

    private async Task AttachCookieAsync(
        AppUser user,
        Team team,
        bool rememberMe,
        string? currentDeviceId = null)
    {
        await _cookieSignInService.SignInWithTwoFactorRequiredAsync(
                isPersistent: rememberMe,
                user: user!,
                team: team!,
                currentDeviceId: currentDeviceId);

    }


    //- - - - - - - - - - - - - - -//


    private async Task<GenResult<MfaResultData>> SendTwoFactoAndAttachCookieAsync(
        AppUser user,
        Team team,
        bool rememberMe,
        string? currentDeviceId = null)
    {
        var tfResultMsg = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);
        if (!tfResultMsg.Succeeded)
            return GenResult<MfaResultData>.Failure(tfResultMsg.Info);

        MfaResultData mfaResultData = tfResultMsg.Value!; //Success is non-null

        await _cookieSignInService.SignInAsync(
                isPersistent: rememberMe,
                user: user!,
                team: team!,
                false,
                currentDeviceId: currentDeviceId);

        return GenResult<MfaResultData>.Success(mfaResultData);
    }


    //- - - - - - - - - - - - - - -//


    private async Task<GenResult<AppUser>> FindOrCreateUserAsync(
        GoogleVerifiedPayload payload,
        GoogleCookieSignInDto dto,
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

