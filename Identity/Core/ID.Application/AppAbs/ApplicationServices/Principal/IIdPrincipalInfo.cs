using ID.Domain.Entities.Teams;
using System.Security.Claims;

namespace ID.Application.AppAbs.ApplicationServices.Principal;

/// <summary>
/// Interface for retrieving information about the logged-in user.
/// </summary>
public interface IIdPrincipalInfo
{
    /// <summary>
    /// Gets the device ID for the specified sub name.
    /// </summary>
    /// <param name="subName">The sub name.</param>
    /// <returns>The device ID.</returns>
    string? DeviceId(string subName);

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    /// <returns>True if the user is authenticated, otherwise false.</returns>
    bool IsAuthenticated();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks if the user is a customer.
    /// </summary>
    /// <returns>True if the user is a customer, otherwise false.</returns>
    bool IsCustomer();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks if the user is a minimum customer.
    /// </summary>
    /// <returns>True if the user is a minimum customer, otherwise false.</returns>
    bool IsCustomerMinimum();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks if the user is in the maintenance team.
    /// </summary>
    /// <returns>True if the user is in the maintenance team, otherwise false.</returns>
    bool IsMntc();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks if the user is in the minimum maintenance team.
    /// </summary>
    /// <returns>True if the user is in the minimum maintenance team, otherwise false.</returns>
    bool IsMntcMinimum();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks if the user is in the super team.
    /// </summary>
    /// <returns>True if the user is in the super team, otherwise false.</returns>
    bool IsSpr();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks if the user is in the minimum super team.
    /// </summary>
    /// <returns>True if the user is in the minimum super team, otherwise false.</returns>
    bool IsSprMinimum();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks if the user is a team leader.
    /// </summary>
    /// <returns>True if the user is a team leader, otherwise false.</returns>
    bool IsLeader();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Gets the team ID of the logged-in user.
    /// </summary>
    /// <returns>The team ID of the logged-in user.</returns>
    Guid? TeamId();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Gets the user ID of the logged-in user.
    /// </summary>
    /// <returns>The user ID of the logged-in user.</returns>
    Guid? UserId();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Gets the email of the logged-in user.
    /// </summary>
    /// <returns>The email of the logged-in user.</returns>
    string? Email();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Gets the username of the logged-in user.
    /// </summary>
    /// <returns>The username of the logged-in user.</returns>
    string? Username();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Gets the team position value of the logged-in user.
    /// </summary>
    /// <returns>The team position value of the logged-in user.</returns>
    int TeamPositionValue();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Gets the team type of the logged-in user.
    /// </summary>
    /// <returns>The team type of the logged-in user.</returns>
    TeamType? TeamType();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Gets the claims principal representing the logged-in user.
    /// </summary>
    /// <returns>The claims principal representing the logged-in user.</returns>
    ClaimsPrincipal? GetPrincipal();

}//Int
