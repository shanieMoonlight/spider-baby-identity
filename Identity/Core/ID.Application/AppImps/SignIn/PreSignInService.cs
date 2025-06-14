using ID.Application.AppAbs.ApplicationServices;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.FromApp;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Features.Account.Cmd.Login;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Application.AppImps.SignIn;


internal class PreSignInService<TUser>(
    IIdUserMgmtService<TUser> _userMgr,
    IFindUserService<TUser> _findUserService,
    IEmailConfirmationBus _emailConfirmationBus,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ITwoFactorMsgService _twoFactorMsgService,
    IIsFromMobileApp _fromAppService) : IPreSignInService<TUser> where TUser : AppUser
{
    public async Task<MyIdSignInResult> Authenticate(LoginDto dto, CancellationToken cancellationToken)
    {
        //Check if user exists
        var user = await _findUserService.FindUserWithTeamDetailsAsync(dto.Email, dto.Username, dto.UserId);
        if (user == null)
            return MyIdSignInResult.NotFoundResult();


        if (!await _userMgr.IsEmailConfirmedAsync(user))
        {
            await _emailConfirmationBus.GenerateTokenAndPublishEventAsync(user, user.Team!, cancellationToken);
            return MyIdSignInResult.EmailConfirmedRequiredResult(user.Email ?? "no-email");
        }


        bool success = await _userMgr.CheckPasswordAsync(user, dto.Password ?? "");
        if (!success)
            return MyIdSignInResult.UnauthorizedResult();


        //Package all user info  and send it back to client.
        var tfEnabled = await _2FactorService.IsTwoFactorEnabledAsync(user);
        if (tfEnabled && !_fromAppService.IsFromApp)
            return await SendTwoFactor(user, user.Team!);


        return MyIdSignInResult.Success(user, user.Team!);
    }


    //-----------------------------//


    private async Task<MyIdSignInResult> SendTwoFactor(AppUser user, Team team)
    {
        var tfResultMsg = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);
        return !tfResultMsg.Succeeded
            ? MyIdSignInResult.Failure(tfResultMsg.Info)
            : MyIdSignInResult.TwoFactorRequiredResult(tfResultMsg.Value, user, team);
    }


    //-----------------------------//


}//Cls
