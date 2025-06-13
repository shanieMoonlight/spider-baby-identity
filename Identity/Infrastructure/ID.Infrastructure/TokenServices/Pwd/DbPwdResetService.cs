using MyResults;
using System.Web;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Infrastructure.TokenServices.Pwd;

internal class DbPwdResetService<TUser>(
    IIdUserMgmtService<TUser> _userMgr,
    IIdentityTeamManager<TUser> _teamMgr,
    IIdUserMgmtUtilityService<TUser> _userUtilityService)
    : IPwdResetService<TUser> where TUser : AppUser
{
    public async Task<BasicResult> ChangePasswordAsync(Team team, TUser user, string password)
    {
        string token = await GenerateSafePasswordResetTokenAsync(team, user);

        return await ResetPasswordAsync(team, user, token, password);
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
        var tkn = await GeneratePasswordResetTokenAsync(team, user);
        return HttpUtility.UrlEncode(tkn);

    }

    //-----------------------------------//

    public async Task<string> GeneratePasswordResetTokenAsync(Team team, TUser user)
    {
        var tkn = await _userMgr.GeneratePasswordResetTokenAsync(user);

        await AddTokenToUser(team, user, tkn);

        return tkn;

    }

    //-----------------------------------//

    public async Task<BasicResult> ResetPasswordAsync(Team team, TUser user, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(newPassword);

        if (user.Tkn != token)
            return BasicResult.Failure(IDMsgs.Error.Tokens.InvalidTkn(nameof(DbPwdResetService<TUser>)));


        var updateResult = await UpdatePasswordHash(user, newPassword, true, cancellationToken);
        if (!updateResult.Succeeded)
            return updateResult;

        //Clear token from DB
        await AddTokenToUser(team, user, null);
        await _teamMgr.SaveChangesAsync(cancellationToken);

        updateResult = await UpdateUserAsync(team, user);
        if (!updateResult.Succeeded)
            return updateResult;

        return BasicResult.Success(IDMsgs.Info.Passwords.PASSWORD_CHANGE_SUCCESSFUL);
    }

    //-----------------------------------//

    private async Task<BasicResult> UpdatePasswordHash(TUser user, string newPassword, bool validatePassword = true, CancellationToken cancellationToken = default)
    {
        if (validatePassword)
        {
            var validate = await _userMgr.ValidatePasswordAsync(user, newPassword);
            if (!validate.Succeeded)
                return validate;
        }

        var hash = newPassword != null
            ? _userUtilityService.PasswordHasher.HashPassword(user, newPassword)
            : null;

        user.PasswordHash = hash;


        return BasicResult.Success();
    }

    //-----------------------------------//

    /// <summary>
    /// Called to update the user after validating and updating the normalized email/user name.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Whether the operation was successful.</returns>
    private async Task<GenResult<TUser>> UpdateUserAsync(Team team, TUser user)
    {
        await _userMgr.UpdateNormalizedUserNameAsync(user);
        await _userMgr.UpdateNormalizedEmailAsync(user);

        return await _teamMgr.UpdateMemberAsync(team, user);
    }

    //-----------------------------------//

    private async Task AddTokenToUser(Team team, TUser user, string? tkn)
    {
        user.SetTkn(tkn);
        await _teamMgr.UpdateMemberAsync(team, user);
        await _teamMgr.SaveChangesAsync();
    }

}//Cls