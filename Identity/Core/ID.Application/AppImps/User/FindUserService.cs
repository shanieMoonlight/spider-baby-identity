using ID.Application.AppAbs.ApplicationServices.User;
using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppImps.User;

/// <summary>
/// Service for finding and retrieving user entities with various search criteria.
/// </summary>
/// <typeparam name="TUser">User type derived from AppUser</typeparam>
internal class FindUserService<TUser>(IIdentityMemberAuditService<AppUser> userMgr)
    : IFindUserService<TUser> where TUser : AppUser
{

    /// <summary>
    /// Finds a user by their unique identifier.
    /// </summary>
    /// <param name="userId">User's unique identifier</param>
    /// <returns>User entity or null if not found</returns>
    public async Task<TUser?> FindUserAsync(Guid? userId) =>
        await userMgr.FirstByIdAsync(userId) as TUser; // TODO: Remove cast when DbContext is fully generic

    //------------------------------------//

    /// <summary>
    /// Finds a user by their email address.
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>User entity or null if not found</returns>
    public async Task<TUser?> FindUserByEmailAsync(string? email) =>
         await userMgr.FirstByEmailAsync(email) as TUser; // TODO: Remove cast when DbContext is fully generic

    //------------------------------------//

    /// <summary>
    /// Finds a user by their username.
    /// </summary>
    /// <param name="username">User's username</param>
    /// <returns>User entity or null if not found</returns>
    public async Task<TUser?> FindUserByUsernameAsync(string? username) =>
         await userMgr.FirstByUsernameAsync(username) as TUser; // TODO: Remove cast when DbContext is fully generic

    //------------------------------------//

    /// <summary>
    /// Finds a user with team details by email, username, or unique identifier.
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="username">User's username</param>
    /// <param name="userId">User's unique identifier</param>
    /// <returns>User entity with team details or null if not found</returns>
    public async Task<TUser?> FindUserWithTeamDetailsAsync(string? email = null, string? username = null, Guid? userId = null)
    {
        if (email is null && userId is null && username is null)
            return null;

        //Check if user exists
        AppUser? user = null;

        if (email is not null)
            user = await userMgr.FirstByEmailWithTeamDetailsAsync(email);

        if (user is null && username is not null)
            user = await userMgr.FirstByUsernameWithTeamDetailsAsync(username);


        if (user is null && userId is not null)
            user = await userMgr.FirstByIdWithTeamDetailsAsync(userId);


        if (user is null && email is not null) //Last check, maybe the email is actually the username
            user = await userMgr.FirstByUsernameWithTeamDetailsAsync(email); 

        return user as TUser; // TODO: Remove cast when DbContext is fully generic
    }

    //------------------------------------//

    /// <summary>
    /// Finds a user with team details by unique identifier.
    /// </summary>
    /// <param name="userId">User's unique identifier</param>
    /// <returns>User entity with team details or null if not found</returns>
    public async Task<TUser?> FindUserWithTeamDetailsAsync(Guid? userId) =>
        await userMgr.FirstByIdWithTeamDetailsAsync(userId) as TUser; // TODO: Remove cast when DbContext is fully generic



}
