using ID.Domain.Entities.SubscriptionPlans;
using ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

namespace ID.Infrastructure.Persistance.Abstractions.Repos;
internal interface IIdentitySubscriptionPlanRepo : IGenCrudRepo<SubscriptionPlan> { }


