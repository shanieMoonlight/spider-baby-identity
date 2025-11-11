using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Repos.GenRepo;

namespace ID.Domain.Repos;
internal interface IIdentityFeatureFlagRepo : IGenCrudRepo<FeatureFlag> { }


