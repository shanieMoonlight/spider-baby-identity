using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Infrastructure.TokenServices.TwoFactor;
internal class DefaultTwoFactorVerificationService<TUser>(IIdUserMgmtService<TUser> _userMgr, IAuthenticatorAppService _authy)
    : BaseTwoFactorVerificationService<TUser>(_userMgr, _authy), ITwoFactorVerificationService<TUser> where TUser : AppUser
{

    //-----------------------//

    public override async Task<string> GenerateTwoFactorTokenAsync(Team team, TUser user, string tokenProvider) =>
        await UserMgr.GenerateTwoFactorTokenAsync(user, tokenProvider);

    //-----------------------//

    public override async Task<bool> VerifyTwoFactorTokenInDbAsync(Team team, TUser user, string token)
    {
        var tokenProvider = await GetFirstValidTwoFactorProviderAsync(user) ?? "Email";
        return await UserMgr.VerifyTwoFactorTokenAsync(user, tokenProvider, token);
    }

    //- - - - - - - - - - - - - - - - - - //

    public override async Task<bool> VerifyTwoFactorTokenInDbAsync(Team team, TUser user, string tokenProvider, string token) =>
        await UserMgr.VerifyTwoFactorTokenAsync(user, tokenProvider, token);


    //-----------------------//

}
