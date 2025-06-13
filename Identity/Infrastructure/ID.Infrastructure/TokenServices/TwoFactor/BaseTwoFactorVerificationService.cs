using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using MyResults;

namespace ID.Infrastructure.TokenServices.TwoFactor;
internal abstract class BaseTwoFactorVerificationService<TUser>(IIdUserMgmtService<TUser> _userMgr, IAuthenticatorAppService authy)
    : ITwoFactorVerificationService<TUser> where TUser : AppUser
{

    protected readonly IIdUserMgmtService<TUser> UserMgr = _userMgr;

    //-----------------------//

    public abstract Task<string> GenerateTwoFactorTokenAsync(Team team, TUser user, string tokenProvider);
    public abstract Task<bool> VerifyTwoFactorTokenInDbAsync(Team team, TUser user, string token);
    public abstract Task<bool> VerifyTwoFactorTokenInDbAsync(Team team, TUser user, string tokenProvider, string token);

    //-----------------------//

    public async Task<bool> IsTwoFactorEnabledAsync(TUser user) =>
        await UserMgr.GetTwoFactorEnabledAsync(user);

    //-----------------------//

    public async Task<bool> VerifyTwoFactorTokenAsync(Team team, TUser user, string token) =>
        user.TwoFactorProvider switch
        {
            //TwoFactorProvider.WhatsApp => await VerifyTwoFactorTokenInDbAsync(user, token),
            TwoFactorProvider.Sms => await VerifyTwoFactorTokenInDbAsync(team, user, token),
            TwoFactorProvider.Email => await VerifyTwoFactorTokenInDbAsync(team, user, token),
            TwoFactorProvider.AuthenticatorApp => await authy.ValidateAsync(user, token),
            //TwoFactorProvider.Authy => await authy.ValidateAsync(user, token),
            _ => false,
        };

    //- - - - - - - - - - - - - - - - - - //

    public async Task<bool> VerifyTwoFactorTokenAsync(Team team, TUser user, TwoFactorProvider tokenProvider, string token) =>
        await UserMgr.VerifyTwoFactorTokenAsync(user, tokenProvider.ToString(), token);

    //-----------------------//

    public async Task<GenResult<TUser>> SetTwoFactorEnabledAsync(TUser user, bool enabled) =>
        await UserMgr.SetTwoFactorEnabledAsync(user, enabled);

    //- - - - - - - - - - - - - - - - - - //

    public async Task<GenResult<TUser>> EnableTwoFactorTokenAsync(TUser user)
    {
        user.Update2FactoEnabled(true);
        return await UserMgr.SetTwoFactorEnabledAsync(user, true);
    }

    //- - - - - - - - - - - - - - - - - - //

    public async Task<GenResult<TUser>> DisableTwoFactorTokenAsync(TUser user)
    {
        user.Update2FactoEnabled(false);
        return await UserMgr.SetTwoFactorEnabledAsync(user, false);
    }

    //-----------------------//

    /// <summary>
    /// Gets the first two factor provider on  for this user.
    /// </summary>
    /// <param name="user">The user the whose two factor authentication providers will be returned.</param>    
    public async Task<string?> GetFirstValidTwoFactorProviderAsync(TUser user) =>
        (await UserMgr.GetValidTwoFactorProvidersAsync(user))
            .OrderBy(vp => vp)
            .FirstOrDefault();

    //-----------------------//

}
