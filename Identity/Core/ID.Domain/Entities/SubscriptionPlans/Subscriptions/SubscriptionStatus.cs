namespace ID.Domain.Entities.SubscriptionPlans.Subscriptions;

public enum SubscriptionStatus
{
    /// <summary>
    ///  The subscription is currently active and the user has full access to the features.
    /// </summary>
    Active = 1,
    /// <summary>
    /// The subscription is not currently active, but it may be reactivated in the future.
    /// </summary>
    InActive = 2,
    /// <summary>
    ///  The subscription has been permanently canceled and the user no longer has access to the features.
    /// </summary>
    Cancelled = 3,
    /// <summary>
    /// The subscription has been temporarily paused and the user's access to features is limited or suspended.
    /// </summary>
    Paused = 4

}//Enm