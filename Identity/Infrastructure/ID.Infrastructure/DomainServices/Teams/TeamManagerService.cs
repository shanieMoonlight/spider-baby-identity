using ClArch.ValueObjects;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Abstractions.Services.Teams.Dvcs;
using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Validators;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Constants;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
using ID.Infrastructure.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Identity;
using MyResults;
using Pagination;


namespace ID.Infrastructure.DomainServices.Teams;

//TODO Remove cast when full generic support is implemented
#pragma warning disable CS8604 // Possible null reference argument.

internal class TeamManagerService<TUser>(
    IIdUnitOfWork uow,
    UserManager<TUser> _userMgr,
    ITeamSubscriptionServiceFactory subsServiceFactory,
    ITeamDeviceServiceFactory dvcServiceFactory)
    : IIdentityTeamManager<TUser> where TUser : AppUser
{
    private readonly IIdentityTeamRepo _teamRepo = uow.TeamRepo;

    //=======================//

    public async Task<IEnumerable<Team>> GetAllTeams(bool includeMntc, bool includeSuper, CancellationToken cancellationToken = default)
    {
        var allTeams = await _teamRepo.ListAllAsync(new AllTeamsSpec(), cancellationToken);

        if (!includeMntc)
            allTeams = [.. allTeams.Where(t => t.TeamType != TeamType.Maintenance)];
        if (!includeSuper)
            allTeams = [.. allTeams.Where(t => t.TeamType != TeamType.Super)];

        return allTeams;
    }

    //- - - - - - - - - - - - - - - - - - //

    public async Task<IReadOnlyList<Team>> GetAllTeamsWithExpiredSubscriptions(CancellationToken cancellationToken) =>
        await _teamRepo.ListAllAsync(new TeamsWithExpiredSubscriptionsSpec(), cancellationToken);

    //- - - - - - - - - - - - - - - - - - //

    public async Task<IEnumerable<Team>> GetCustomerTeamsByNameAsync(string? name) =>
        await _teamRepo.ListAllAsync(new CustomerTeamsByNameSpec(name));

    //- - - - - - - - - - - - - - - - - - //

    public async Task<Page<Team>> GetPageAsync(PagedRequest pgRequest) =>
        await GetPageAsync(pgRequest.PageNumber, pgRequest.PageSize, pgRequest.SortList, pgRequest.FilterList);

    public async Task<Page<Team>> GetPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest> filterList) =>
        await _teamRepo.PageAsync(pageNumber, pageSize, sortList, filterList);

    //=======================//

    public async Task<GenResult<ITeamSubscriptionService>> GetSubscriptionServiceAsync(Guid? teamId) =>
        await subsServiceFactory.GetServiceAsync(teamId);

    //- - - - - - - - - - - - - - - - - - //

    public async Task<GenResult<ITeamSubscriptionService>> GetSubscriptionServiceAsync(Team team) =>
        await subsServiceFactory.GetServiceAsync(team);

    //- - - - - - - - - - - - - - - - - - //

    public async Task<GenResult<ITeamDeviceService>> GetDeviceServiceAsync(Guid? teamId, Guid? subId) =>
        await dvcServiceFactory.GetServiceAsync(teamId, subId);

    //=======================//

    public async Task<Team?> GetById_NoDetails_Async(Guid? teamId) =>
        await _teamRepo.FirstOrDefaultAsync(new GetByIdSpec<Team>(teamId));

    //- - - - - - - - - - - - - - - - - - //

    public async Task<Team?> GetByIdWithMemberAsync(Guid? teamId, Guid? userId) =>
        await _teamRepo.FirstOrDefaultAsync(new TeamByIdWithMemberSpec(teamId, userId));

    //- - - - - - - - - - - - - - - - - - //

    public async Task<Team?> GetByIdWithMembersAsync(Guid? teamId, int maxPosition = IdGlobalConstants.Teams.DEFAULT_MAX_POSITION) =>
        await _teamRepo.FirstOrDefaultAsync(new TeamByIdWithMembersSpec(teamId, maxPosition));

    //- - - - - - - - - - - - - - - - - - //

    public async Task<Team?> GetSuperTeamWithMembersAsync(int maxPosition = IdGlobalConstants.Teams.DEFAULT_MAX_POSITION) =>
        await _teamRepo.FirstOrDefaultAsync(new SuperTeamWithMembersSpec(maxPosition));

    public async Task<Team?> GetSuperTeamWithMemberAsync(Guid? userId, int maxPosition = IdGlobalConstants.Teams.DEFAULT_MAX_POSITION) =>
        await _teamRepo.FirstOrDefaultAsync(new SuperTeamWithMemberSpec(userId, maxPosition));

    //- - - - - - - - - - - - - - - - - - //

    public async Task<Team?> GetMntcTeamWithMembersAsync(int maxPosition = IdGlobalConstants.Teams.DEFAULT_MAX_POSITION) =>
        await _teamRepo.FirstOrDefaultAsync(new MntcTeamWithMembersSpec(maxPosition));

    public async Task<Team?> GetMntcTeamWithMemberAsync(Guid? userId, int maxPosition = IdGlobalConstants.Teams.DEFAULT_MAX_POSITION) =>
        await _teamRepo.FirstOrDefaultAsync(new MntcTeamWithMemberSpec(userId, maxPosition));

    //- - - - - - - - - - - - - - - - - - //

    public async Task<Team?> GetByIdWithSubscriptionsAsync(Guid? teamId) =>
        await _teamRepo.FirstOrDefaultAsync(new TeamByIdWithSubscriptionsSpec(teamId));

    //- - - - - - - - - - - - - - - - - - //

    public async Task<Team?> GetByIdWithEverythingAsync(Guid? teamId, int maxPosition = IdGlobalConstants.Teams.DEFAULT_MAX_POSITION) =>
        await _teamRepo.FirstOrDefaultAsync(new TeamByIdWithEverythingSpec(teamId, maxPosition));

    //-----------------------//

    public async Task<Team> AddTeamAsync(Team newTeam, CancellationToken cancellationToken)
    {
        var dbTeam = await _teamRepo.AddAsync(newTeam, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return dbTeam;
    }

    //-----------------------//

    public async Task<Team> UpdateAsync(Team team, Name name, DescriptionNullable description)
    {
        var updatedTeam = team.Update(name, description);

        await UpdateAndSaveAsync(updatedTeam);

        return updatedTeam;
    }

    //-----------------------//

    public async Task<Team> UpdateAsync(Team updatedTeam) =>
        await UpdateAndSaveAsync(updatedTeam);


    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await uow.SaveChangesAsync(cancellationToken);


    private async Task<Team> UpdateAndSaveAsync(Team team)
    {
        var updatedTeam = await _teamRepo.UpdateAsync(team);
        await uow.SaveChangesAsync();
        return updatedTeam;
    }

    //-----------------------//

    public async Task<BasicResult> DeleteTeamAsync(Team team)
    {
        var canDeleteResult = await CanDeleteAsync(team.Id);
        if (!canDeleteResult.Succeeded)
            return canDeleteResult;

        foreach (var mbr in team.Members)
        {
            var validationResult = RemoveMember(team, mbr as TUser);
            if (!validationResult.Succeeded)
                return validationResult;

            var deleteResult = await _userMgr.DeleteAsync(mbr as TUser);
            if (!deleteResult.Succeeded)
                return deleteResult.ToBasicResult();
        }

        await _teamRepo.DeleteAsync(team);
        await uow.SaveChangesAsync();

        return BasicResult.Success();
    }

    //=======================//

    public async Task<GenResult<TUser>> RegisterMemberAsync(Team team, TUser newMember)
    {
        // Step 1: Validate using domain validator
        var validationResult = TeamManagerService<TUser>.TryAddMember(team, newMember);
        if (!validationResult.Succeeded)
            return validationResult.Convert<TUser>();


        // Step 3: Infrastructure operations
        var createResult = await CreateUserAsync(newMember);
        if (!createResult.Succeeded)
            return createResult;

        // Step 4: Persist changes
        await UpdateAndSaveAsync(team);
        return GenResult<TUser>.Success(newMember);
    }

    //- - - - - - - - - - - -//

    public async Task<GenResult<TUser>> RegisterMemberWithPasswordAsync(Team team, TUser newMember, string password)
    {
        if (password == null)
            return GenResult<TUser>.Failure(IDMsgs.Error.MISSING_DATA(nameof(password)));


        var validationResult = TeamManagerService<TUser>.TryAddMember(team, newMember);
        if (!validationResult.Succeeded)
            return validationResult.Convert<TUser>();


        var createResult = await CreateUserWithPasswordAsync(newMember, password);
        if (!createResult.Succeeded)
            return createResult;

        await UpdateAndSaveAsync(team);
        return GenResult<TUser>.Success(newMember);
    }

    //- - - - - - - - - - - -//

    private static GenResult<TeamValidators.MemberAddition.Token> TryAddMember(Team team, TUser newMember)
    {
        var validationResult = TeamValidators.MemberAddition.Validate(team, newMember);
        if (!validationResult.Succeeded)
            return validationResult;

        var validationToken = validationResult.Value!; // Success is non-null

        // Step 2: Domain operation (guaranteed safe)
        team.AddMember(validationToken);
        return validationResult;
    }


    //-----------------------//

    public async Task<GenResult<TUser>> UpdateMemberAsync(Team team, TUser updateUser)
    {
        var dbUser = await GetTeamMemberAsync(team, updateUser.Id);

        if (dbUser is null || !IsTeamMember(team, updateUser))
            return GenResult<TUser>.Failure(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(updateUser, team));

        //Can't change Leader position
        if (updateUser.TeamPosition != dbUser.TeamPosition)
        {
            if (team.LeaderId == dbUser.Id)
                return GenResult<TUser>.Failure(IDMsgs.Error.Teams.CANT_CHANGE_POSITION_OF_LEADER);
        }

        if (dbUser.TwoFactorEnabled != updateUser.TwoFactorEnabled)
            await _userMgr.SetTwoFactorEnabledAsync(dbUser, updateUser.TwoFactorEnabled);

        //Call MicrosoftUserManger in case Something needs to happen with stores etc.
        var updateResult = await _userMgr.UpdateAsync(updateUser);
        if (!updateResult.Succeeded)
            return updateResult.ToGenResult(updateUser);

        return GenResult<TUser>.Success(dbUser);
    }

    //-----------------------// 

    public async Task<BasicResult> DeleteMemberAsync(Team team, Guid memberId)
    {
        var dbUser = await GetTeamMemberAsync(team, memberId);
        if (dbUser is null)
            return GenResult<TUser>.NotFoundResult(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(memberId, team.Name));

        //Active teams can't be empty. Delete the team instead (If it's a Customer team) .
        if (dbUser.Id == team.LeaderId)
            return GenResult<TUser>.BadRequestResult(IDMsgs.Error.Teams.CANT_DELETE_LEADER);

        var validationResult = RemoveMember(team, dbUser);
        if (!validationResult.Succeeded)
            return validationResult;

        var deleteResult = await _userMgr.DeleteAsync(dbUser);

        await UpdateAndSaveAsync(team);
        return deleteResult.ToBasicResult();
    }

    //-----------------------//

    /// <summary>
    /// Tries to remove the specified <paramref name="dbUser"/> from the <paramref name="team"/>.  
    /// Returns a validation token if successful, or an error result if not.
    /// </summary>
    private static GenResult<TeamValidators.MemberRemoval.Token> RemoveMember(Team team, TUser dbUser)
    {

        var validationResult = TeamValidators.MemberRemoval.Validate(team, dbUser);
        if (!validationResult.Succeeded)
            return validationResult;

        var validationToken = validationResult.Value!; // Success is non-null
        team.RemoveMember(validationToken);

        return validationResult;
    }

    //-----------------------//


    public async Task<TUser?> GetMemberAsync(Guid? teamId, Guid? userId)
    {
        if (teamId is null || userId is null)
            return null;

        var team = await _teamRepo.FirstOrDefaultAsync(new TeamByIdWithMemberSpec(teamId, userId));
        return team?.Members.FirstOrDefault(u => u.Id == userId) as TUser;
    }

    //-----------------------//

    public async Task<IList<TUser>> GetAllMembers(Team team, int maxPosition = IdGlobalConstants.Teams.DEFAULT_MAX_POSITION)
    {
        var dbTeam = await _teamRepo.FirstOrDefaultAsync(new TeamByIdWithMembersSpec(team.Id, maxPosition));

        return dbTeam?.Members.Cast<TUser>().ToList() ?? [];
    }


    //-----------------------//

    public async Task<GenResult<Team>> SetLeaderAsync(Team team, TUser newLeader)
    {
        // Step 1: Validate using domain validator
        var validationResult = TeamValidators.LeaderUpdate.Validate(team, newLeader);
        if (!validationResult.Succeeded)
            return validationResult.Convert<Team>();

        var validationToken = validationResult.Value!; // Success is non-null


        team.SetLeader(validationToken);

        var updatedTeam = await UpdateAndSaveAsync(team);

        return GenResult<Team>.Success(updatedTeam);
    }

    //=======================//

    private async Task<TUser?> GetTeamMemberAsync(Team team, Guid memberId)
    {

        var member = team.Members.FirstOrDefault(u => u.Id == memberId);
        if (member is not null)
            return member as TUser;

        var allMembers = await GetAllMembers(team);
        return allMembers.FirstOrDefault(u => u.Id == memberId);
    }

    //-----------------------//

    /// <summary>
    /// Creates the specified <paramref name="user"/> in the backing store with no password,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult"/>
    /// of the operation.
    /// </returns>
    private async Task<GenResult<TUser>> CreateUserAsync(TUser user)
    {

        var createResult = await _userMgr.CreateAsync(user);

        if (!createResult.Succeeded)
            return createResult.ToGenResult<TUser>(user);

        return GenResult<TUser>.Success(user);
    }

    //-----------------------//

    /// <summary>
    /// Creates the specified <paramref name="user"/> in the backing store with given password,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <param name="password">The password for the user to hash and store.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult"/>
    /// of the operation.
    /// </returns>
    private async Task<GenResult<TUser>> CreateUserWithPasswordAsync(TUser user, string password)
    {

        var createResult = await _userMgr.CreateAsync(user, password);

        if (!createResult.Succeeded)
            return createResult.ToGenResultFailure<TUser>();

        return GenResult<TUser>.Success(user);

    }

    //-----------------------//

    private static bool IsTeamMember(Team team, TUser? newMember) =>
        team.Id == newMember?.TeamId;

    //-----------------------//

    private async Task<BasicResult> CanDeleteAsync(Guid teamId)
    {

        var dbTeam = await GetByIdWithMembersAsync(teamId);
        if (dbTeam == null)
            return BasicResult.NotFoundResult(IDMsgs.Error.NotFound<Team>(teamId));

        return TeamManagerService<TUser>.CanDeleteAsync(dbTeam);
    }


    private static BasicResult CanDeleteAsync(Team dbTeam)
    {
        if (dbTeam.TeamType != TeamType.Customer)
            return BasicResult.BadRequestResult(IDMsgs.Error.Teams.CAN_ONLY_REMOVE_CUSTOMER_TEAM);

        var nonLeaderMemberCount = dbTeam.Members
            ?.Where(m => m.Id != dbTeam.LeaderId)
            ?.Count() ?? 0;
        var isAre = nonLeaderMemberCount > 1 ? "are" : "is";
        var mbr = nonLeaderMemberCount > 1 ? "members" : "member";

        return nonLeaderMemberCount > 0
            ? BasicResult.BadRequestResult($"There {isAre} {nonLeaderMemberCount} {mbr} connected to Team, {dbTeam.Name}. You must delete them or move them to a different Team before deleting this Team.")
            : BasicResult.Success();
    }


}//Cls
