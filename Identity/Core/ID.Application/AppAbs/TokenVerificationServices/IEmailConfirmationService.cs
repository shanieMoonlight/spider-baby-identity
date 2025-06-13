using MyResults;
using Microsoft.AspNetCore.Identity;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppAbs.TokenVerificationServices;

public interface IEmailConfirmationService<TUser> where TUser : AppUser
{
    //------------------------------------//

    /// <summary>
    /// Validates that an email confirmation token matches the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to validate the token against.</param>
    /// <param name="confirmationToken">The email confirmation token to validate.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public Task<BasicResult> ConfirmEmailAsync(Team team, TUser user, string confirmationToken);


    /// <summary>
    /// Validates that an email confirmation token matches the specified <paramref name="user"/>.
    /// Adds the password to the user.
    /// USe this when users are added by an admin and not the user themselves. (For new team members)
    /// </summary>
    /// <param name="user">The user to validate the token against.</param>
    /// <param name="confirmationToken">The email confirmation token to validate.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    Task<BasicResult> ConfirmEmailWithPasswordAsync(Team team, TUser user, string confirmationToken, string password);

    //------------------------------------//

    /// <summary>
    /// Generates a email confirmation token for the specified user. Not UrlEncoded
    /// </summary>
    /// <param name="user">The user to generate an email confirmation token for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, email confirmation token.
    /// </returns>
    Task<string> GenerateEmailConfirmationTokenAsync(Team team, TUser user);

    //------------------------------------//

    /// <summary>
    /// Generates an UrlEncoded email confirmation token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate an email confirmation token for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, an UrlEncodedd  email confirmation token.
    /// </returns>
    Task<string> GenerateSafeEmailConfirmationTokenAsync(Team team, TUser user);

    //------------------------------------//

}//Cls