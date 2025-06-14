using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.Account.Cmd.Cookies.TwoFactorVerifyCookie;
public class Verify2FactorCookieCmdHandler(
    ICookieSignInService<AppUser> _cookieSignInService,
    IFindUserService<AppUser> _findUserService,
    ITwoFactorVerificationService<AppUser> _2FactorService)
    : IIdCommandHandler<Verify2FactorCookieCmd>
{

    public async Task<BasicResult> Handle(Verify2FactorCookieCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
      
        var userId = request.PrincipalUserId ?? dto.UserId; //Two factor failure may have returned a Jwt or Cookie. If not clien cust supply an ID
        var user = await _findUserService.FindUserWithTeamDetailsAsync(userId: userId);
        var team = user?.Team;


        if (user is null || team is null)
            return BasicResult.UnauthorizedResult(IDMsgs.Error.Authorization.INVALID_AUTH);


        bool validVerification = await _2FactorService.VerifyTwoFactorTokenAsync(team, user, dto.Token);
        if (!validVerification)
            return BasicResult.BadRequestResult(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN);


        //Attach cookie
        await _cookieSignInService.SignInAsync(
           dto.RememberMe,
           user!,
           team!,
           false,
           dto.DeviceId);

        return BasicResult.Success("Signed In!");

    }


}//Cls
