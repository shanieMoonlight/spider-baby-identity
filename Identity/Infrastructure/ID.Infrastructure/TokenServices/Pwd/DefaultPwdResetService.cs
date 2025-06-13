using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using MyResults;
using System.Web;

namespace ID.Infrastructure.TokenServices.Pwd;

internal class DefaultPwdResetService<TUser>(IIdUserMgmtService<TUser> userMgr) 
    : IPwdResetService<TUser> where TUser : AppUser
{
    //-----------------------------------//  

    public async Task<BasicResult> ChangePasswordAsync(Team team, TUser user, string password)
    {
        string token = await GenerateSafePasswordResetTokenAsync(team, user);

        var resetResult = await ResetPasswordAsync(team, user, token, password);

        return resetResult;
    }

    //-----------------------//

    /// <summary>
    /// Generates an UrlEncoded password reset token for the specified <paramref name="user"/>, using
    /// the configured password reset token provider.
    /// </summary>
    /// <param name="user">The user to generate a password reset token for.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation,
    /// containing an UrlEncoded password reset token for the specified <paramref name="user"/>.</returns>
    public async Task<string> GenerateSafePasswordResetTokenAsync(Team team, TUser user)
    {
        var tkn = await userMgr.GeneratePasswordResetTokenAsync(user);
        return HttpUtility.UrlEncode(tkn);
    }

    //-----------------------------------//

    public async Task<string> GeneratePasswordResetTokenAsync(Team team, TUser user) =>
      await userMgr.GeneratePasswordResetTokenAsync(user);

    //-----------------------------------//

    public async Task<BasicResult> ResetPasswordAsync(Team team, TUser user, string token, string newPassword, CancellationToken cancellationToken = default) => 
        await userMgr.ResetPasswordAsync(user, token, newPassword);

    //-----------------------------------//

}//Cls