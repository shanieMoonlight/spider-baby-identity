using MyResults;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppAbs.TokenVerificationServices;
/// <summary>
/// Provides services for phone number confirmation operations.
/// </summary>
/// <typeparam name="TUser">The user type that inherits from AppUser.</typeparam>
public interface IPhoneConfirmationService<TUser> where TUser : AppUser
{
    /// <summary>
    /// Confirms a user's phone number using a verification token.
    /// </summary>
    /// <param name="team">The team the user belongs to.</param>
    /// <param name="user">The user whose phone number is being confirmed.</param>
    /// <param name="confirmationToken">The token to validate the phone number change.</param>
    /// <param name="newPhone">The new phone number to confirm.</param>
    /// <returns>
    /// A <see cref="BasicResult"/> that represents the outcome of the operation.
    /// Returns success if the phone number was confirmed, otherwise failure.
    /// </returns>
    Task<BasicResult> ConfirmPhoneAsync(Team team, TUser user, string confirmationToken, string newPhone);

    //-------------------------------------//

    /// <summary>
    /// Generates a token for confirming a change to a user's phone number.
    /// </summary>
    /// <param name="team">The team the user belongs to.</param>
    /// <param name="user">The user whose phone number is being changed.</param>
    /// <param name="newPhoneNumber">The new phone number to be confirmed.</param>
    /// <returns>
    /// A token string that can be used to confirm the phone number change.
    /// </returns>
    Task<string> GeneratePhoneChangedConfirmationTokenAsync(Team team, TUser user, string newPhoneNumber);

    //-------------------------------------//

    /// <summary>
    /// Checks if a user's phone number has been confirmed.
    /// </summary>
    /// <param name="user">The user to check.</param>
    /// <returns>
    /// True if the user's phone number is confirmed, otherwise false.
    /// </returns>
    Task<bool> IsPhoneConfirmedAsync(TUser user);

}//Int
