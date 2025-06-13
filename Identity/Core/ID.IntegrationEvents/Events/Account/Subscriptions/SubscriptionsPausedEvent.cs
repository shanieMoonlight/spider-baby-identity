using ID.IntegrationEvents.Abstractions;

namespace ID.IntegrationEvents.Events.Account.Subscriptions;

public record SubscriptionsPausedIntegrationEvent : AIdIntegrationEvent
{
    public Guid LeaderId { get; set; }
    public Guid SubscriptionId { get; set; }
    public string Email { get; set; }
    public string ToName { get; set; }
    public string SubscriptionPlanName { get; set; }

    //------------------------//

    #region MassTransitCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Required for MassTransit. Do not use.
    /// </summary>
    public SubscriptionsPausedIntegrationEvent() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    public SubscriptionsPausedIntegrationEvent(Guid leaderId, Guid subId, string email, string toName, string subscriptionPlanName)
    {
        LeaderId = leaderId;
        SubscriptionId = subId;
        Email = email;
        ToName = toName;
        SubscriptionPlanName = subscriptionPlanName;
    }

    //------------------------//

}//Cls
