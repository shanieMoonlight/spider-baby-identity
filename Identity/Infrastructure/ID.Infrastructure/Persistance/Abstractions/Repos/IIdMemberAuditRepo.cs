using ClArch.SimpleSpecification;
using ID.Domain.Entities.AppUsers;
using Pagination;

namespace ID.Infrastructure.Persistance.Abstractions.Repos;
/// <summary>
/// Repository for managing members in the identity system.
/// </summary>
internal interface IIdentityMemberAuditRepo<TUser> where TUser : AppUser
{

    /// <summary>
    /// Checks if a member with the specified email exists.
    /// </summary>
    /// <param name="email">The email of the member.</param>
    /// <returns>True if the member exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(ASimpleSpecification<TUser> specification, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves the first entity has an id = <paramref name="id"/>.
    /// </summary>
    /// <returns>The entity if it exists</returns>
    Task<TUser?> FirstOrDefaultByIdAsync(Guid? id);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Retrieves the first entity that matches the specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if it exists</returns>
    Task<TUser?> FirstOrDefaultAsync(ASimpleSpecification<TUser> specification, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a list of all customer team members.
    /// </summary>
    /// <returns>A paginated list of customer team members.</returns>
    Task<IReadOnlyList<TUser>> GetAllCustomersAsync();

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves a paginated list of customer team members.
    /// </summary>
    /// <param name="request">The pagination request details.</param>
    /// <returns>A paginated list of customer team members.</returns>
    Task<Page<TUser>> GetCustomerPageAsync(PagedRequest request);

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves a paginated list of customer team members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortList">The list of sorting criteria.</param>
    /// <param name="filterList">The list of filtering criteria.</param>
    /// <returns>A paginated list of customer team members.</returns>
    Task<Page<TUser>> GetCustomerPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList);

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves a list of all maintenance team members with position no greater than <paramref name="maxPosition"/>.
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <returns>A list of maintenance team members.</returns>
    Task<IReadOnlyList<TUser>> GetAllMntcMembersAsync(int maxPosition = 1000);

    //- - - - - - - - - - - - - -//
    /// <summary>
    /// Retrieves a paginated list of maintenance team members.
    /// </summary>
    /// <param name="request">The pagination request details.</param>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <returns>A paginated list of maintenance team members.</returns>
    Task<Page<TUser>> GetMntcPageAsync(PagedRequest request, int maxPosition = 1000);

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves a paginated list of maintenance team members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortList">The list of sorting criteria.</param>
    /// <param name="filterList">The list of filtering criteria.</param>
    /// <returns>A paginated list of maintenance team members.</returns>
    Task<Page<TUser>> GetMntcPageAsync(int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList);

    //- - - - - - - - - - - - - -//

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
    Task<Page<TUser>> GetMembersPageAsync(Guid teamId, int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList);

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves a list of all super team members, with position no greater than <paramref name="maxPosition"/>..
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <returns>A list of super team members.</returns>
    Task<IReadOnlyList<TUser>> GetAllSuperMembersAsync(int maxPosition = 1000);

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves a paginated list of super team members.
    /// </summary>
    /// <param name="request">The pagination request details.</param>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <returns>A paginated list of super team members.</returns>
    Task<Page<TUser>> GetSuperPageAsync(PagedRequest request, int maxPosition);

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves a paginated list of super team members.
    /// </summary>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortList">The list of sorting criteria.</param>
    /// <param name="filterList">The list of filtering criteria.</param>
    /// <returns>A paginated list of super team members.</returns>
    Task<Page<TUser>> GetSuperPageAsync(int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList);

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves a paginated list of members from a specific team.
    /// </summary>
    /// <param name="request">The pagination request details.</param>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// <returns>A paginated list of team members.</returns>
    Task<Page<TUser>> GetMembersPageAsync(PagedRequest request, Guid teamId, int maxPosition);

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves a member from a specific team.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <param name="mbrId">The ID of the member.</param>
    /// <returns>The member if found; otherwise, null.</returns>
    Task<TUser?> GetMemberAsync(Guid? teamId, Guid? mbrId);

    //- - - - - - - - - - - - - -//

    /// <summary>
    /// Retrieves a list of all entities that match the specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all entities that match the specification</returns>
    Task<IReadOnlyList<TUser>> ListAllAsync(ASimpleSpecification<TUser> specification, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - - - - - - //

}//Int

