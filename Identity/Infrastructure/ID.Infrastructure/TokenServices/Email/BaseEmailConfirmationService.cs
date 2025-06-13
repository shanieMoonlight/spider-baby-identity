using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using MyResults;
using System.Web;

namespace ID.Infrastructure.TokenServices.Email;
internal abstract class BaseEmailConfirmationService<TUser>(IIdUserMgmtService<TUser> _userMgr, IPwdResetService<TUser> _pwdReset) 
    : IEmailConfirmationService<TUser> where TUser : AppUser
{
    protected readonly IIdUserMgmtService<TUser> UserMgr = _userMgr;

    //-----------------------//

    public abstract Task<BasicResult> ConfirmEmailAsync(Team team, TUser user, string token);

    //-----------------------//

    public async Task<BasicResult> ConfirmEmailWithPasswordAsync(Team team, TUser user, string confirmationToken, string password)
    {
        var confirmResult = await ConfirmEmailAsync(team, user, confirmationToken);
        if (!confirmResult.Succeeded)
            return confirmResult;

        //This is a bit of  a hack but it's the only way to set the password after the user was created
        string token = await _pwdReset.GeneratePasswordResetTokenAsync(team, user);
        var resetResult = await _pwdReset.ResetPasswordAsync(team, user, token, password);
        if (!resetResult.Succeeded)
            return resetResult;

        return BasicResult.Success(IDMsgs.Info.Email.EMAIL_CONFIRMED);
    }

    //-----------------------//

    public abstract Task<string> GenerateEmailConfirmationTokenAsync(Team team, TUser user);

    //-----------------------//

    public virtual async Task<string> GenerateSafeEmailConfirmationTokenAsync(Team team, TUser user)
    {
        var tkn = await UserMgr.GenerateEmailConfirmationTokenAsync(user);
        return HttpUtility.UrlEncode(tkn);
    }

    //-----------------------//

}//Cls
