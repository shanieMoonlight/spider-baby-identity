using ID.Domain.Entities.OutboxMessages;
using ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

namespace ID.Infrastructure.Persistance.Abstractions.Repos;
internal interface IIdentityOutboxMessageRepo
    : IGenReadRepo<IdOutboxMessage>,
     IGenUpdateRepo<IdOutboxMessage>,
     IGenDeleteRepo<IdOutboxMessage> //Create will happen in the DbInterceptors
{


}//Cls
