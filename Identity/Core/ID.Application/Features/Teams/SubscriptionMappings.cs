using ID.Domain.Entities.Teams;

namespace ID.Application.Features.Teams;

public static class SubscriptionMappings
{

    public static SubscriptionDto ToDto(this TeamSubscription mdl) => new(mdl);


}//Cls


