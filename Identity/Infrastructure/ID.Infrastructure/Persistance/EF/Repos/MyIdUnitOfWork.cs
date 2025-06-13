using ID.Application.AppAbs.RequestInfo;
using ID.Domain.Abstractions.Services.Transactions;
using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.DomainServices.Transactions;
using ID.Infrastructure.Persistance.Abstractions.Repos;

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

    public async Task<IIdTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var dbContextTransaction = await db.Database.BeginTransactionAsync(cancellationToken);
        return new IdTransaction(dbContextTransaction);
    }

    //-----------------------//

}//Cls
