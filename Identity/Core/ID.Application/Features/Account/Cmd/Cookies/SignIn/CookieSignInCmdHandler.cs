using ID.Application.AppAbs.SignIn;
using ID.Application.Dtos.Account.Cookies;
using ID.Application.Mediatr.CqrsAbs;
using ID.Application.Utility.ExtensionMethods;
using ID.Domain.Entities.AppUsers;
using MyResults;


namespace ID.Application.Features.Account.Cmd.Cookies.SignIn;
public class CookieSignInCmdHandler(
    IPreSignInService<AppUser> _preSignInService,
    ICookieAuthService<AppUser> _cookieSignInService)
    : IIdCommandHandler<CookieSignInCmd, CookieSignInResultData>
{

    public async Task<GenResult<CookieSignInResultData>> Handle(CookieSignInCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var signInResult = await _preSignInService.Authenticate(dto, cancellationToken);
        var cookieSignInResultData = signInResult.ToCookieSignInResultData();

        if (signInResult.TwoFactorRequired)
        {
            //Attach cookie
            await _cookieSignInService.CreateWithTwoFactorRequiredAsync(
               dto.RememberMe,
               signInResult.User!,
               dto.DeviceId);
            return signInResult.ToGenResult(cookieSignInResultData);
        }

        if (signInResult.Succeeded)
        {
            //Attach cookie
            await _cookieSignInService.SignInAsync(
               dto.RememberMe,
               signInResult.User!,
               signInResult.Team!,
               dto.DeviceId);
            return signInResult.ToGenResult(cookieSignInResultData);
        }


        return signInResult.ToGenResult(cookieSignInResultData);
    }


}//Cls

