using ID.Domain.Entities.Teams;
using ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

namespace ID.Infrastructure.Persistance.Abstractions.Repos;
internal interface IIdentitySubscriptionRepo : IGenCrudRepo<TeamSubscription> { }


