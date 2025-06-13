using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.Utility.ExtensionMethods;
using MyResults;

namespace ID.Infrastructure.TokenServices.Email;
internal class DefaultEmailConfirmationService<TUser>(IIdUserMgmtService<TUser> _userMgr, IPwdResetService<TUser> _pwdReset)
    : BaseEmailConfirmationService<TUser>(_userMgr, _pwdReset), IEmailConfirmationService<TUser> where TUser : AppUser
{
    public override async Task<BasicResult> ConfirmEmailAsync(Team team, TUser user, string token)
    {
        var result = await UserMgr.ConfirmEmailAsync(user, token);

        return result.Succeeded
           ? result.ToBasicResult(IDMsgs.Info.Email.EMAIL_CONFIRMED)
           : result.ToBasicResult();
    }

    //-----------------------//

    public override Task<string> GenerateEmailConfirmationTokenAsync(Team team, TUser user) =>
        UserMgr.GenerateEmailConfirmationTokenAsync(user);

    //-----------------------//


}//Cls
