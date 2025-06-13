using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Members;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Members.WithEverything;
using Pagination;

namespace ID.Infrastructure.DomainServices.Members;
internal class IdMemberAuditService<TUser>(IIdentityMemberAuditRepo<TUser> _repo)
    : IIdentityMemberAuditService<TUser> where TUser : AppUser
{
    //-----------------------//

    /// <summary>
    /// Checks if an User/Team Member with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the User.</param>
    /// <returns>True if the User exists, otherwise false.</returns>
    public Task<bool> ExistsAsync(Guid? id) =>
        _repo.ExistsAsync(new MemberExistsIdSpec<TUser>(id));

    //- - - - - - - - - - - -// 

    /// <summary>
    /// Checks if an User/Team Member with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the User.</param>
    /// <returns>True if the User exists, otherwise false.</returns>
    public Task<bool> ExistsAsync(string? email) =>
        _repo.ExistsAsync(new MemberExistsEmailSpec<TUser>(email));

    //-----------------------//

    public Task<TUser?> GetMemberAsync(Guid? teamId, Guid? mbrId) =>
        _repo.GetMemberAsync(teamId, mbrId);

    //-----------------------//

    public Task<Page<TUser>> GetCustomerPageAsync(PagedRequest request) =>
        _repo.GetCustomerPageAsync(request);

    //- - - - - - - - - - - - - -//

    public Task<Page<TUser>> GetCustomerPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList) =>
        _repo.GetCustomerPageAsync(pageNumber, pageSize, sortList, filterList);

    //- - - - - - - - - - - - - -//

    public Task<IReadOnlyList<TUser>> GetCustomersAsync() =>
        _repo.GetAllCustomersAsync();

    //-----------------------//

    public Task<Page<TUser>> GetMembersPageAsync(Guid teamId, int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null) =>
        _repo.GetMembersPageAsync(teamId, maxPosition, pageNumber, pageSize, sortList, filterList);

    //-----------------------//

    public Task<Page<TUser>> GetMembersPageAsync(PagedRequest request, Guid teamId, int maxPosition) =>
        _repo.GetMembersPageAsync(request, teamId, maxPosition);

    //-----------------------//

    public Task<Page<TUser>> GetMntcPageAsync(PagedRequest request, int maxPosition = 1000) =>
        _repo.GetMntcPageAsync(request);

    //-----------------------//

    public Task<Page<TUser>> GetMntcPageAsync(int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null) =>
        _repo.GetMntcPageAsync(maxPosition, pageNumber, pageSize, sortList, filterList);

    //-----------------------//

    public Task<IReadOnlyList<TUser>> GetMntcMembersAsync(int maxPosition) =>
        _repo.GetAllMntcMembersAsync(maxPosition);

    //-----------------------//

    public Task<Page<TUser>> GetSuperPageAsync(PagedRequest request, int maxPosition) =>
        _repo.GetSuperPageAsync(request, maxPosition);

    //-----------------------//

    public Task<Page<TUser>> GetSuperPageAsync(int maxPosition, int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null) =>
        _repo.GetSuperPageAsync(maxPosition, pageNumber, pageSize, sortList, filterList);

    //-----------------------//

    public Task<IReadOnlyList<TUser>> GetSuperMembersAsync(int maxPosition) =>
        _repo.GetAllSuperMembersAsync(maxPosition);

    //-----------------------//
    public Task<TUser?> FirstByEmailAsync(string? email) =>
        _repo.FirstOrDefaultAsync(new MemberByEmailSpec<TUser>(email));

    //-----------------------//

    public Task<TUser?> FirstByUsernameAsync(string? username) =>
        _repo.FirstOrDefaultAsync(new MemberByUsernameSpec<TUser>(username));

    //- - - - - - - - - - - -//

    public Task<TUser?> FirstByEmailWithTeamDetailsAsync(string? email) =>
        _repo.FirstOrDefaultAsync(new MemberByEmailWithEverythingSpec<TUser>(email));

    //- - - - - - - - - - - -//

    public Task<TUser?> FirstByIdWithTeamDetailsAsync(Guid? id) =>
        _repo.FirstOrDefaultAsync(new MemberByIdWithEverythingSpec<TUser>(id));

    //- - - - - - - - - - - -//

    public Task<TUser?> FirstByIdAsync(Guid? id) =>
        _repo.FirstOrDefaultByIdAsync(id);

    //- - - - - - - - - - - -//

    public Task<TUser?> FirstByUsernameWithTeamDetailsAsync(string? username) =>
        _repo.FirstOrDefaultAsync(new MemberByUsernameWithEverythingSpec<TUser>(username));

    //-----------------------//

}//Cls
