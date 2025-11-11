using ID.Application.AppAbs.RequestInfo;
using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.DomainServices.Transactions;
using ID.Domain.Repos;
using ID.Domain.Repos.Transactions;

namespace ID.Infrastructure.Persistance.EF.Repos;
internal class MyIdUnitOfWork(
    IUserInfo userInfo,
    IdDbContext db,
    IIdentityTeamRepo teamRepo,
    IIdentityMemberAuditRepo<AppUser> memberRepo,
    IIdentitySubscriptionPlanRepo subscriptionDefinitionRepo,
    IIdentityFeatureFlagRepo featureFlagRepo,
    IIdentityOutboxMessageRepo outboxMessageRepo,
    IIdentityRefreshTokenRepo refreshTokenRepo)
    : IIdUnitOfWork
{

    //-----------------------//

    public IIdentityTeamRepo TeamRepo => teamRepo;
    public IIdentityOutboxMessageRepo OutboxMessageRepo => outboxMessageRepo;
    public IIdentitySubscriptionPlanRepo SubscriptionPlanRepo => subscriptionDefinitionRepo;
    public IIdentityFeatureFlagRepo FeatureFlagRepo => featureFlagRepo;
    public IIdentityMemberAuditRepo<AppUser> MemberRepo => memberRepo;
    public IIdentityRefreshTokenRepo RefreshTokenRepo => refreshTokenRepo;

    //-----------------------//

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    //-----------------------//

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await db.SaveChangesAsync(userInfo.GetLoggedInUserName(), userInfo.GetLoggedInUserId(), cancellationToken);

    //-----------------------//

    public Task<IIdExecutionStrategy> CreateExecutionStrategy()
    {
        var dbContextExecutionStrategy = db.Database.CreateExecutionStrategy();
        var adapter = new EfCoreExecutionStrategyAdapter(dbContextExecutionStrategy);
        return Task.FromResult<IIdExecutionStrategy>(adapter);
    }
    
    //- - - - - - - - - - - -//

    public async Task<IIdTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var dbContextTransaction = await db.Database.BeginTransactionAsync(cancellationToken);
        return new IdTransaction(dbContextTransaction);
    }

}//Cls
