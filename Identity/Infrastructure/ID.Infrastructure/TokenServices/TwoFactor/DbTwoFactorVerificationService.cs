using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Infrastructure.TokenServices.TwoFactor;
internal class DbTwoFactorVerificationService<TUser>(
    IIdUserMgmtService<TUser> _userMgr,
    IIdentityTeamManager<TUser> _teamMgr,
    IAuthenticatorAppService _authy)
    : BaseTwoFactorVerificationService<TUser>(_userMgr, _authy),
    ITwoFactorVerificationService<TUser> where TUser : AppUser
{
    //----------------------------------//

    public override async Task<string> GenerateTwoFactorTokenAsync(Team team, TUser user, string tokenProvider)
    {
        var tkn = await UserMgr.GenerateTwoFactorTokenAsync(user, tokenProvider);
        await AddTokenToUser(team, user, tkn);

        return tkn;
    }

    //----------------------------------//

    public override async Task<bool> VerifyTwoFactorTokenInDbAsync(Team team, TUser user, string token)
    {
        if (user.Tkn != token)
            return false;

        var isValid = user.Tkn == token;
        await AddTokenToUser(team, user, null);

        return isValid;
    }

    //- - - - - - - - - - - - - - - - - - //

    public override async Task<bool> VerifyTwoFactorTokenInDbAsync(Team team, TUser user, string tokenProvider, string token) =>
        await UserMgr.VerifyTwoFactorTokenAsync(user, tokenProvider, token);


    //-----------------------//

    private async Task AddTokenToUser(Team team, TUser user, string? tkn)
    {
        user.SetTkn(tkn);
        await _teamMgr.UpdateMemberAsync(team, user);
        await _teamMgr.SaveChangesAsync();
    }

    //----------------------------------//

}
