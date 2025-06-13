using ClArch.SimpleSpecification;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using Microsoft.EntityFrameworkCore;
using Pagination;
using Pagination.Extensions;
using StringHelpers;

namespace ID.Infrastructure.Persistance.EF.Repos;
/// <summary>
/// Repository for viewing Team Members/AppUsers.
/// </summary>
internal class MemberAuditRepo(IdDbContext Db) : IIdentityMemberAuditRepo<AppUser>
{
    public async Task<bool> ExistsAsync(ASimpleSpecification<AppUser> spec, CancellationToken cancellationToken = default)
    {
        if (spec.ShouldShortCircuit())
            return false;

        return await Db.Users
            .BuildQuery(spec)
            .AnyAsync(cancellationToken);
    }

    //-----------------------//

    public async Task<AppUser?> FirstOrDefaultByIdAsync(Guid? id) =>
        await Db.Users.FindAsync(id);

    //-----------------------//

    /// <summary>
    /// Retrieves all customer team members.
    /// </summary>
    /// <returns>A list of customer team members.</returns>
    public async Task<IReadOnlyList<AppUser>> GetAllCustomersAsync() =>
        await GetAllTeamMembersAsync(TeamType.Customer);

    //- - - - - - - - - - - - - - - - - - // 

    /// <summary>
    /// Retrieves all maintenance team members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <returns>A list of maintenance team members.</returns>
    public async Task<IReadOnlyList<AppUser>> GetAllMntcMembersAsync(int maxPosition = 1000) =>
        await GetAllTeamMembersAsync(TeamType.Maintenance, maxPosition);

    //- - - - - - - - - - - - - - - - - - // 

    /// <summary>
    /// Retrieves all super team members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <returns>A list of super team members.</returns>
    public async Task<IReadOnlyList<AppUser>> GetAllSuperMembersAsync(int maxPosition = 1000) =>
        await GetAllTeamMembersAsync(TeamType.Super, maxPosition);


    //- - - - - - - - - - - - - - - - - - // 

    public async Task<IReadOnlyList<AppUser>> GetAllTeamMembersAsync(TeamType teamType, int maxPosition = 1000) =>
        await Db.Teams
            .Include(team => team.Members)
            .Where(t => t.TeamType == teamType)
            .SelectMany(t => t.Members)
            .Where(m => m.TeamPosition <= maxPosition)
            .ToListAsync();

    //-----------------------//

    /// <summary>
    /// Retrieves a member from a specific team.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="mbrId">The ID of the member.</param>
    /// <returns>The member if found; otherwise, null.</returns>
    public async Task<AppUser?> GetMemberAsync(Guid? teamId, Guid? mbrId)
    {
        if (!teamId.HasValue || !mbrId.HasValue)
            return null;

        var team = await Db.Teams
            .Include(b => b.Members.Where(m => m.Id == mbrId))
            .FirstOrDefaultAsync(b => b.Id == teamId);

        return team?.Members?.FirstOrDefault();
    }

    //-----------------------//

    /// <summary>
    /// Retrieves a paginated list of customer team members.
    /// </summary>
    /// <param name="request">The pagination request details.</param>
    /// <returns>A paginated list of customer team members.</returns>
    public async Task<Page<AppUser>> GetCustomerPageAsync(PagedRequest request) =>
      await GetCustomerPageAsync(request.PageNumber, request.PageSize, request.SortList, request.FilterList);

    //- - - - - - - - - - - - - - - - - - // 

    /// <summary>
    /// Retrieves a paginated list of customer team members.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortList">The list of sorting criteria.</param>
    /// <param name="filterList">The list of filtering criteria.</param>
    /// <returns>A paginated list of customer team members.</returns>
    public Task<Page<AppUser>> GetCustomerPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList) =>
        GetTeamMembersPageAsync(TeamType.Customer, pageNumber, pageSize, sortList, filterList); //Position = 10000 is a hack to get all customers. Only Mntc or above will get here 

    //-----------------------//

    /// <summary>
    /// Retrieves a paginated list of maintenance team members.
    /// </summary>
    /// <param name="request">The pagination request details.</param>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <returns>A paginated list of maintenance team members.</returns>
    public async Task<Page<AppUser>> GetMntcPageAsync(PagedRequest request, int maxPosition = 1000) =>
      await GetMntcPageAsync(maxPosition, request.PageNumber, request.PageSize, request.SortList, request.FilterList);

    //- - - - - - - - - - - - - - - - - - // 

    /// <summary>
    /// Retrieves a paginated list of maintenance team members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortList">The list of sorting criteria.</param>
    /// <param name="filterList">The list of filtering criteria.</param>
    /// <returns>A paginated list of maintenance team members.</returns>
    public Task<Page<AppUser>> GetMntcPageAsync(int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList) =>
        GetTeamMembersPageAsync(TeamType.Maintenance, maxPosition, pageNumber, pageSize, sortList, filterList);

    //-----------------------//

    /// <summary>
    /// Retrieves a paginated list of super team members.
    /// </summary>
    /// <param name="request">The pagination request details.</param>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <returns>A paginated list of super team members.</returns>
    public async Task<Page<AppUser>> GetSuperPageAsync(PagedRequest request, int maxPosition) =>
      await GetSuperPageAsync(maxPosition, request.PageNumber, request.PageSize, request.SortList, request.FilterList);

    //-----------------------//

    /// <summary>
    /// Retrieves a paginated list of super team members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortList">The list of sorting criteria.</param>
    /// <param name="filterList">The list of filtering criteria.</param>
    /// <returns>A paginated list of super team members.</returns>
    public Task<Page<AppUser>> GetSuperPageAsync(int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList) =>
        GetTeamMembersPageAsync(TeamType.Super, maxPosition, pageNumber, pageSize, sortList, filterList);

    //-----------------------//

    /// <summary>
    /// Retrieves a paginated list of members from a specific team.
    /// </summary>
    /// <param name="request">The pagination request details.</param>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <returns>A paginated list of team members.</returns>
    public async Task<Page<AppUser>> GetMembersPageAsync(PagedRequest request, Guid teamId, int maxPosition) =>
      await GetMembersPageAsync(teamId, maxPosition, request.PageNumber, request.PageSize, request.SortList, request.FilterList);

    //- - - - - - - - - - - - - - - - - - // 

    /// <summary>
    /// Retrieves a paginated list of members from a specific team.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortList">The list of sorting criteria.</param>
    /// <param name="filterList">The list of filtering criteria.</param>
    /// <returns>A paginated list of team members.</returns>
    public Task<Page<AppUser>> GetMembersPageAsync(Guid teamId, int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList)
    {
        var pages = Db.Teams
            .Include(b => b.Members)
            .Where(t => t.Id == teamId)
            .SelectMany(t => t.Members)
            .Where(m => m.TeamPosition <= maxPosition)
            .AddFiltering(filterList, GetMembersFilteringPropertySelectorLambda)
            .AddEfSorting(sortList, GetSortingExpBuider());

        var page = new Page<AppUser>(pages, pageNumber, pageSize);
        return Task.FromResult(page);
    }

    //-----------------------//

    public async Task<AppUser?> FirstOrDefaultAsync(ASimpleSpecification<AppUser> spec, CancellationToken cancellationToken = default)
    {
        if (spec.ShouldShortCircuit())
            return null;

        return await Db.Set<AppUser>()
            .BuildQuery(spec)
            .FirstOrDefaultAsync(cancellationToken);
    }

    //-----------------------//

    public async Task<IReadOnlyList<AppUser>> ListAllAsync(ASimpleSpecification<AppUser> spec, CancellationToken cancellationToken = default)
    {
        if (spec.ShouldShortCircuit())
            return [];

        return await Db.Set<AppUser>()
            .BuildQuery(spec)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    //-----------------------//

    /// <summary>
    /// Retrieves a paginated list of super team members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortList">The list of sorting criteria.</param>
    /// <param name="filterList">The list of filtering criteria.</param>
    /// <returns>A paginated list of super team members.</returns>
    private Task<Page<AppUser>> GetTeamMembersPageAsync(
        TeamType teamType,
        int pageNumber,
        int pageSize,
        IEnumerable<SortRequest> sortList,
        IEnumerable<FilterRequest>? filterList)
    {
        var pages = Db.Teams
            .Include(b => b.Members)
            .Where(t => t.TeamType == teamType)
            .SelectMany(t => t.Members)
            .AddFiltering(filterList, GetMembersFilteringPropertySelectorLambda)
            .AddEfSorting(sortList, GetSortingExpBuider());

        var page = new Page<AppUser>(pages, pageNumber, pageSize);
        return Task.FromResult(page);
    }

    //-----------------------//

    /// <summary>
    /// Retrieves a paginated list of super team members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortList">The list of sorting criteria.</param>
    /// <param name="filterList">The list of filtering criteria.</param>
    /// <returns>A paginated list of super team members.</returns>
    private Task<Page<AppUser>> GetTeamMembersPageAsync(
        TeamType teamType,
        int maxPosition,
        int pageNumber,
        int pageSize,
        IEnumerable<SortRequest> sortList,
        IEnumerable<FilterRequest>? filterList)
    {
        var pages = Db.Teams
            .Include(b => b.Members)
            .Where(t => t.TeamType == teamType)
            .SelectMany(t => t.Members)
            .Where(m => m.TeamPosition <= maxPosition)
            .AddFiltering(filterList, GetMembersFilteringPropertySelectorLambda)
            .AddEfSorting(sortList, GetSortingExpBuider());

        var page = new Page<AppUser>(pages, pageNumber, pageSize);
        return Task.FromResult(page);
    }

    //-----------------------//

    /// <summary>
    /// Converts a field name from camel case to pascal case.
    /// </summary>
    /// <param name="field">The field name in camel case.</param>
    /// <returns>The field name in pascal case.</returns>
    private string GetMembersFilteringPropertySelectorLambda(string field) =>
        field.CamelToPascal();

    //- - - - - - - - - - - - - - - - - - // 

    /// <summary>
    /// Gets the custom sort expression builder for AppUser.
    /// </summary>
    /// <returns>The custom sort expression builder.</returns>
    protected virtual CustomSortExpressionBuilder<AppUser>? GetSortingExpBuider() => null;

    //-----------------------//

}//Cls
