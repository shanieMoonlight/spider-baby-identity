using ID.Domain.Abstractions.Services.Transactions;
using ID.Domain.Entities.AppUsers;

namespace ID.Infrastructure.Persistance.Abstractions.Repos;
internal interface IIdUnitOfWork : IDisposable
{
    IIdentityTeamRepo TeamRepo { get; }
    IIdentityRefreshTokenRepo RefreshTokenRepo { get; }
    IIdentitySubscriptionPlanRepo SubscriptionPlanRepo { get; }
    IIdentityFeatureFlagRepo FeatureFlagRepo { get; }
    IIdentityOutboxMessageRepo OutboxMessageRepo { get; }
    IIdentityMemberAuditRepo<AppUser> MemberRepo { get; }


    Task<IIdTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

}//int
