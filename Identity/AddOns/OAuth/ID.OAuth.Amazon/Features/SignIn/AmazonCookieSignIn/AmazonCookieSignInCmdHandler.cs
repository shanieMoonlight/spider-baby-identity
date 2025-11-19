using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Dtos.Account.Cookies;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.OAuth.Amazon.Services.Abs;
using MyResults;

namespace ID.OAuth.Amazon.Features.SignIn.AmazonCookieSignIn;

public class AmazonCookieSignInCmdHandler(
    IFindOrCreateService<AppUser> _findOrCreate,
    ICookieAuthService<AppUser> _cookieSignInService,
    IAmazonAuthenticationService _verifier,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ITwoFactorMsgService _twoFactorMsgService)
    : IIdCommandHandler<AmazonCookieSignInCmd, CookieSignInResultData>
{
    public async Task<GenResult<CookieSignInResultData>> Handle(AmazonCookieSignInCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var profileResult = await _verifier.VerifyAndGetProfileAsync(dto.AuthToken, dto.Id, cancellationToken);
        if (!profileResult.Succeeded)
            return profileResult.Convert<CookieSignInResultData>();

        var userProfile = profileResult.Value!;
        var userResult = await _findOrCreate.FindOrCreateUserAsync(userProfile, dto, cancellationToken);
        if (!userResult.Succeeded)
            return userResult.Convert<CookieSignInResultData>();

        var user = userResult.Value!; var team = user.Team!;

        var twoFactorEnabled = await _2FactorService.IsTwoFactorEnabledAsync(user);

        return twoFactorEnabled
            ? await ReturnTwoFactorCookieAsync(user, team, dto.RememberMe, dto.DeviceId)
            : await ReturnStandardCookieAsync(user, team, dto.RememberMe, dto.DeviceId);
    }

    private async Task<GenResult<CookieSignInResultData>> ReturnStandardCookieAsync(AppUser user, Team team, bool rememberMe, string? currentDeviceId)
    {
        await _cookieSignInService.SignInAsync(isPersistent: rememberMe, user: user!, team: team!, currentDeviceId: currentDeviceId);
        return GenResult<CookieSignInResultData>.Success(CookieSignInResultData.Success());
    }

    private async Task<GenResult<CookieSignInResultData>> ReturnTwoFactorCookieAsync(AppUser user, Team team, bool rememberMe, string? currentDeviceId)
    {
        var twoFactorResult = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);
        if (!twoFactorResult.Succeeded)
            return GenResult<CookieSignInResultData>.Failure(twoFactorResult.Info);

        var mfa = twoFactorResult.Value!;

        await _cookieSignInService.CreateWithTwoFactorRequiredAsync(isPersistent: rememberMe, user: user!, currentDeviceId);

        var data = CookieSignInResultData.CreateWithTwoFactoRequired(provider: mfa.TwoFactorProvider, message: ID.Domain.Utility.Messages.IDMsgs.Error.Authorization.TWO_FACTOR_REQUIRED(mfa.TwoFactorProvider));
        return GenResult<CookieSignInResultData>.PreconditionRequiredResult(data);
    }
}
