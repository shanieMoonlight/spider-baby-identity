using ID.Domain.Entities.OutboxMessages;
using ID.Domain.Repos.GenRepo;

namespace ID.Domain.Repos;
internal interface IIdentityOutboxMessageRepo
    : IGenReadRepo<IdOutboxMessage>,
     IGenUpdateRepo<IdOutboxMessage>,
     IGenDeleteRepo<IdOutboxMessage> //Create will happen in the DbInterceptors
{


}//Cls
