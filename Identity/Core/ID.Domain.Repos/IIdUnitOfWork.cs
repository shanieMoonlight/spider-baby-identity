using ID.Domain.Entities.AppUsers;
using ID.Domain.Repos.Transactions;

namespace ID.Domain.Repos;
internal interface IIdUnitOfWork : IDisposable
{
    IIdentityTeamRepo TeamRepo { get; }
    IIdentityRefreshTokenRepo RefreshTokenRepo { get; }
    IIdentitySubscriptionPlanRepo SubscriptionPlanRepo { get; }
    IIdentityFeatureFlagRepo FeatureFlagRepo { get; }
    IIdentityOutboxMessageRepo OutboxMessageRepo { get; }
    IIdentityMemberAuditRepo<AppUser> MemberRepo { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IIdExecutionStrategy> CreateExecutionStrategy();
    Task<IIdTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);



}//int
