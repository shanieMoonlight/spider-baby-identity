using ClArch.ValueObjects;
using ID.Domain.Abstractions.Services.Teams.Dvcs;
using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using MyResults;
using Pagination;

namespace ID.Domain.Abstractions.Services.Teams;

/// <summary>
/// Interface for managing identity teams.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
public interface IIdentityTeamManager<TUser> where TUser : AppUser
{
    //================================================//

    /// <summary>
    /// Gets the subscription service for a team by its ID.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subscription service.</returns>
    Task<GenResult<ITeamSubscriptionService>> GetSubscriptionServiceAsync(Guid? teamId);

    /// <summary>
    /// Gets the subscription service for a team.
    /// </summary>
    /// <param name="team">The team entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subscription service.</returns>
    Task<GenResult<ITeamSubscriptionService>> GetSubscriptionServiceAsync(Team team);

    /// <summary>
    /// Gets the device service for a team by its ID and subscription ID.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="subId">The ID of the subscription.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the device service.</returns>
    Task<GenResult<ITeamDeviceService>> GetDeviceServiceAsync(Guid? teamId, Guid? subId);

    //================================================//

    /// <summary>
    /// Gets a team by its ID without details.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the team entity.</returns>
    Task<Team?> GetById_NoDetails_Async(Guid? teamId);

    /// <summary>
    /// Gets a team by its ID with members.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="maxPosition">The maximum position of members to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the team entity.</returns>
    Task<Team?> GetByIdWithMembersAsync(Guid? teamId, int maxPosition = 1000);

    /// <summary>
    /// Gets a team by its ID with all details.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="maxPosition">The maximum position of members to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the team entity.</returns>
    Task<Team?> GetByIdWithEverythingAsync(Guid? teamId, int maxPosition = 1000);

    /// <summary>
    /// Gets a team by its ID with a specific member.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the team entity.</returns>
    Task<Team?> GetByIdWithMemberAsync(Guid? teamId, Guid? userId);

    /// <summary>
    /// Gets a team by its ID with subscriptions.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the team entity.</returns>
    Task<Team?> GetByIdWithSubscriptionsAsync(Guid? teamId);

    //- - - - - - - - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Gets the super team with members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of members to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the team entity.</returns>
    Task<Team?> GetSuperTeamWithMembersAsync(int maxPosition = 1000);

    /// <summary>
    /// Gets the super team with a specific member.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="maxPosition">The maximum position of members to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the team entity.</returns>
    Task<Team?> GetSuperTeamWithMemberAsync(Guid? userId, int maxPosition = 1000);

    /// <summary>
    /// Gets the maintenance team with members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of members to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the team entity.</returns>
    Task<Team?> GetMntcTeamWithMembersAsync(int maxPosition = 1000);

    /// <summary>
    /// Gets the maintenance team with a specific member.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="maxPosition">The maximum position of members to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the team entity.</returns>
    Task<Team?> GetMntcTeamWithMemberAsync(Guid? userId, int maxPosition = 1000);

    //- - - - - - - - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Adds a new team.
    /// </summary>
    /// <param name="newTeam">The new team entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added team entity.</returns>
    Task<Team> AddTeamAsync(Team newTeam, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a team.
    /// </summary>
    /// <param name="team">The team entity to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the deletion.</returns>
    Task<BasicResult> DeleteTeamAsync(Team team);

    /// <summary>
    /// Updates a team.
    /// </summary>
    /// <param name="team">The team entity to update.</param>
    /// <param name="name">The new name of the team.</param>
    /// <param name="description">The new description of the team.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated team entity.</returns>
    Task<Team> UpdateAsync(Team team, Name name, DescriptionNullable description);

    //================================================//

    /// <summary>
    /// Deletes a member from a team.
    /// </summary>
    /// <param name="team">The team entity.</param>
    /// <param name="memberId">The ID of the member to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the deletion.</returns>
    Task<BasicResult> DeleteMemberAsync(Team team, Guid memberId);

    /// <summary>
    /// Gets all members of a team.
    /// </summary>
    /// <param name="team">The team entity.</param>
    /// <param name="maxPosition">The maximum position of members to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of members.</returns>
    Task<IList<TUser>> GetAllMembers(Team team, int maxPosition = 1000);

    /// <summary>
    /// Gets a member of a team by their ID.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the member entity.</returns>
    Task<TUser?> GetMemberAsync(Guid? teamId, Guid? userId);

    /// <summary>
    /// Registers a new member to a team.
    /// </summary>
    /// <param name="team">The team entity.</param>
    /// <param name="newMember">The new member entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the registration.</returns>
    Task<GenResult<TUser>> RegisterMemberAsync(Team team, TUser newMember);

    /// <summary>
    /// Registers a new member to a team with a password.
    /// </summary>
    /// <param name="team">The team entity.</param>
    /// <param name="newMember">The new member entity.</param>
    /// <param name="password">The password for the new member.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the registration.</returns>
    Task<GenResult<TUser>> RegisterMemberWithPasswordAsync(Team team, TUser newMember, string password);

    /// <summary>
    /// Sets a new leader for a team.
    /// </summary>
    /// <param name="team">The team entity.</param>
    /// <param name="newLeader">The new leader entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
    Task<GenResult<Team>> SetLeaderAsync(Team team, TUser newLeader);

    /// <summary>
    /// Updates a member of a team.
    /// </summary>
    /// <param name="team">The team entity.</param>
    /// <param name="updateUser">The member entity to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the update.</returns>
    Task<GenResult<TUser>> UpdateMemberAsync(Team team, TUser updateUser);

    /// <summary>
    /// Updates a team.
    /// </summary>
    /// <param name="updatedTeam">The updated team entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated team entity.</returns>
    Task<Team> UpdateAsync(Team updatedTeam);

    /// <summary>
    /// Gets all teams.
    /// </summary>
    /// <param name="includeMntc">Whether to include maintenance teams.</param>
    /// <param name="includeSuper">Whether to include super teams.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of teams.</returns>
    Task<IEnumerable<Team>> GetAllTeams(bool includeMntc, bool includeSuper, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all teams with expired subscriptions.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of teams with expired subscriptions.</returns>
    Task<IReadOnlyList<Team>> GetAllTeamsWithExpiredSubscriptions(CancellationToken cancellationToken);


    /// <summary>
    /// Gets customer teams by their name.
    /// </summary>
    /// <param name="name">The name of the customer teams.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of customer teams.</returns>
    Task<IEnumerable<Team>> GetCustomerTeamsByNameAsync(string? name);

    /// <summary>
    /// Gets a page of teams.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="sortList">The list of sort requests.</param>
    /// <param name="filterList">The list of filter requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the page of teams.</returns>
    Task<Page<Team>> GetPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest> filterList);

    /// <summary>
    /// Gets a page of teams.
    /// </summary>
    /// <param name="pgRequest">The paged request.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the page of teams.</returns>
    Task<Page<Team>> GetPageAsync(PagedRequest pgRequest);


    Task SaveChangesAsync(CancellationToken cancellationToken = default);


}//Cls
