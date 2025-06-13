using ID.Domain.Entities.Teams;

namespace ID.Domain.Utility.Exceptions;

public class TeamDoesNotContainSubscriptionException(Team team, Guid subscriptionId)
    : MyIdException($"The Team {team.Name} does not contain a Subscription with Id {subscriptionId}")
{ }