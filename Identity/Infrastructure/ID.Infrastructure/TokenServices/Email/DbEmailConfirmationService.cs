using MyResults;
using System.Web;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Infrastructure.TokenServices.Email;
internal class DbEmailConfirmationService<TUser>(
    IIdUserMgmtService<TUser> _userMgr,
    IIdentityTeamManager<TUser> _teamMgr,
    IPwdResetService<TUser> _pwdReset)
    : BaseEmailConfirmationService<TUser>(_userMgr, _pwdReset), IEmailConfirmationService<TUser> where TUser : AppUser
{
    //-----------------------//

    public override async Task<BasicResult> ConfirmEmailAsync(Team team, TUser user, string confirmationToken)
    {
        if (user.Tkn != confirmationToken)
            return BasicResult.Failure(IDMsgs.Error.Tokens.InvalidTkn(nameof(DbEmailConfirmationService<TUser>)));

        await SetEmailConfirmedAsync(team, user);
        return BasicResult.Success(IDMsgs.Info.Email.EMAIL_CONFIRMED);
    }

    //-----------------------//

    public override async Task<string> GenerateEmailConfirmationTokenAsync(Team team, TUser user)
    {
        var tkn = await UserMgr.GenerateEmailConfirmationTokenAsync(user);
        await AddTokenToUser(team, user, tkn);
        return tkn;
    }

    //-----------------------//

    public override async Task<string> GenerateSafeEmailConfirmationTokenAsync(Team team, TUser user)
    {
        var tkn = await UserMgr.GenerateEmailConfirmationTokenAsync(user);
        await AddTokenToUser(team, user, tkn);
        return HttpUtility.UrlEncode(tkn);
    }

    //-----------------------//

    private async Task SetEmailConfirmedAsync(Team team, TUser user)
    {
        user.EmailConfirmed = true;
        await AddTokenToUser(team, user, null);
    }

    //-----------------------//

    private async Task AddTokenToUser(Team team, TUser user, string? tkn)
    {
        user.SetTkn(tkn);
        await _teamMgr.UpdateMemberAsync(team, user);
        await _teamMgr.SaveChangesAsync();
    }

    //-----------------------//

}//Cls
