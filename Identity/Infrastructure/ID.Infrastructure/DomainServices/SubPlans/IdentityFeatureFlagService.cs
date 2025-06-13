using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;
using ID.Infrastructure.Persistance.EF.Repos.Specs.FeatureFlags;
using Pagination;

namespace ID.Infrastructure.DomainServices.SubPlans;

internal class IdentityFeatureFlagService(IIdentityFeatureFlagRepo _repo) : IIdentityFeatureFlagService
{
    public Task<FeatureFlag> AddAsync(FeatureFlag entity, CancellationToken cancellationToken = default)
        => _repo.AddAsync(entity, cancellationToken);

    //- - - - - - - - - - - - - - - - - - //

    public Task AddRangeAsync(IEnumerable<FeatureFlag> entities, CancellationToken cancellationToken = default)
        => _repo.AddRangeAsync(entities, cancellationToken);

    //- - - - - - - - - - - - - - - - - - //

    public Task<int> CountAsync()
        => _repo.CountAsync();

    //- - - - - - - - - - - - - - - - - - //

    public Task DeleteAsync(FeatureFlag entity)
        => _repo.DeleteAsync(entity);

    //- - - - - - - - - - - - - - - - - - //

    public Task DeleteAsync(Guid? id)
        => _repo.DeleteAsync(id);

    //- - - - - - - - - - - - - - - - - - //

    public Task<bool> ExistsAsync(Guid? id)
        => _repo.ExistsAsync(id);

    //- - - - - - - - - - - - - - - - - - //

    public Task<IReadOnlyList<FeatureFlag>> GetAllAsync()
        => _repo.ListAllAsync();

    //- - - - - - - - - - - - - - - - - - //

    public Task<FeatureFlag?> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default)
        => _repo.FirstOrDefaultAsync(new GetByIdSpec<FeatureFlag>(id), cancellationToken);

    //- - - - - - - - - - - - - - - - - - //

    public async Task<FeatureFlag?> GetByIdWithPlansAsync(Guid? id)=> 
        await _repo.FirstOrDefaultAsync(new FlagByIdWithPlansSpec(id));

    //- - - - - - - - - - - - - - - - - - //

    public async Task<FeatureFlag?> GetByNameAsync(string? name, CancellationToken cancellationToken) => 
        await _repo.FirstOrDefaultAsync(new FlagByNameSpec(name), cancellationToken);

    //- - - - - - - - - - - - - - - - - - //

    public async Task<IReadOnlyList<FeatureFlag>> GetAllByNameAsync(string? name, CancellationToken cancellationToken) =>
        await _repo.ListAllAsync(new FlagByNameSpec(name), cancellationToken);

    //- - - - - - - - - - - - - - - - - - //

    public Task<Page<FeatureFlag>> GetPageAsync(PagedRequest request)
        => _repo.PageAsync(request);

    //- - - - - - - - - - - - - - - - - - //

    public Task<Page<FeatureFlag>> GetPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null)
        => _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

    //- - - - - - - - - - - - - - - - - - //

    public Task<FeatureFlag> UpdateAsync(FeatureFlag entity)
        => _repo.UpdateAsync(entity);

    //- - - - - - - - - - - - - - - - - - //

    public Task UpdateRangeAsync(IEnumerable<FeatureFlag> entities)
        => _repo.UpdateRangeAsync(entities);

}//Cls
