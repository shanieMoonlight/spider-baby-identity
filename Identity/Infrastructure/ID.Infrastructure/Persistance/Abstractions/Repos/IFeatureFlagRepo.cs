using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

namespace ID.Infrastructure.Persistance.Abstractions.Repos;
internal interface IIdentityFeatureFlagRepo : IGenCrudRepo<FeatureFlag> { }


