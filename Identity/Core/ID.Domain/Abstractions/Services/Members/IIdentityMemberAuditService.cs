using ID.Domain.Entities.AppUsers;
using Pagination;

namespace ID.Domain.Abstractions.Services.Members;
public interface IIdentityMemberAuditService<TUser> where TUser : AppUser
{
    /// <summary>
    /// Check if a member exists by email
    /// </summary>
    Task<bool> ExistsAsync(string? email);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Check if a member exists by ID
    /// </summary>
    Task<bool> ExistsAsync(Guid? id);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Find the first user with the given email, if Exists.
    /// Includes all related entities. , Team, Subs, Devices, SubPlans etc.
    /// </summary>
    Task<TUser?> FirstByEmailWithTeamDetailsAsync(string? email);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Find the first user with the given email, if Exists.
    /// Includes NO related entities.
    /// </summary>
    Task<TUser?> FirstByEmailAsync(string? email);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Find the first user with the given username, if Exists.
    /// Includes NO related entities.
    /// </summary>
    Task<TUser?> FirstByUsernameAsync(string? username);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Find the first user with the given id, if Exists.
    /// Includes NO related entities.
    /// </summary>
    Task<TUser?> FirstByIdAsync(Guid? id);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Find the first user with the given id, if Exists.
    /// Includes all related entities. , Team, Subs, Devices, SubPlans etc.
    /// </summary>
    Task<TUser?> FirstByIdWithTeamDetailsAsync(Guid? id);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Find the first user with the given username, if Exists.
    /// Includes all related entities. , Team, Subs, Devices, SubPlans etc.
    /// </summary>
    Task<TUser?> FirstByUsernameWithTeamDetailsAsync(string? username);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get all Customer Members
    /// </summary>
    Task<IReadOnlyList<TUser>> GetCustomersAsync();

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get Customer Members Paged
    /// </summary>
    Task<Page<TUser>> GetCustomerPageAsync(PagedRequest request);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get Customer Members Paged
    /// </summary>
    Task<Page<TUser>> GetCustomerPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get Member by Team ID and Member ID
    /// </summary>
    Task<TUser?> GetMemberAsync(Guid? teamId, Guid? mbrId);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get all Mntc Members
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// </summary>
    Task<IReadOnlyList<TUser>> GetMntcMembersAsync(int maxPosition);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get Mntc Members Paged
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// </summary>
    Task<Page<TUser>> GetMntcPageAsync(PagedRequest request, int maxPosition = 1000);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get Mntc Members Paged
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// </summary>
    Task<Page<TUser>> GetMntcPageAsync(int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get Members from Team with id, <paramref name="teamId"/> Paged
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// </summary>
    Task<Page<TUser>> GetMembersPageAsync(PagedRequest request, Guid teamId, int maxPosition);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get Members from Team with id, <paramref name="teamId"/> Paged
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// </summary>
    Task<Page<TUser>> GetMembersPageAsync(Guid teamId, int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get all Super Members
    /// </summary>
    Task<IReadOnlyList<TUser>> GetSuperMembersAsync(int maxPosition);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get Super Members Paged
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// </summary>
    Task<Page<TUser>> GetSuperPageAsync(PagedRequest request, int maxPosition = 1000);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get Super Members Paged
    /// <param name="maxPosition">The maximum position of the members.</param>
    /// </summary>
    Task<Page<TUser>> GetSuperPageAsync(int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null);
    
}//Int
