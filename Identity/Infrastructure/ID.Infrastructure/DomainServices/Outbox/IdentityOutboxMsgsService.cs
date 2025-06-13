using ID.Domain.Abstractions.Services.Outbox;
using ID.Domain.Entities.OutboxMessages;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.OutboxMsgs;
using Pagination;

namespace ID.Infrastructure.DomainServices.Outbox;
internal class IdentityOutboxMsgsService(IIdentityOutboxMessageRepo repo) : IIdentityOutboxMsgsService
{
    //- - - - - - - - - - - - - - - - - - //

    public Task<IReadOnlyList<IdOutboxMessage>> GetAllAsync() 
        => repo.ListAllAsync();

    //- - - - - - - - - - - - - - - - - - //

    public Task<IdOutboxMessage?> GetByIdAsync(Guid? id)
        => repo.FirstOrDefaultByIdAsync(id);

    //- - - - - - - - - - - - - - - - - - //

    public Task<IReadOnlyList<IdOutboxMessage>> GetAllByTypeAsync(string? name) 
        => repo.ListAllAsync(new OutboxMsgsByTypeSpec(name));

    //- - - - - - - - - - - - - - - - - - //
    public Task<Page<IdOutboxMessage>> GetPageAsync(PagedRequest request) 
        => repo.PageAsync(request);

    //- - - - - - - - - - - - - - - - - - //

    public Task<Page<IdOutboxMessage>> GetPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null) 
        => repo.PageAsync(pageNumber, pageSize, sortList, filterList);

}//Cls
