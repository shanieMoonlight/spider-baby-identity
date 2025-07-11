using ID.Domain.Entities.Teams;

namespace ID.Application.JWT.Subscriptions;

/// <summary>
/// Represents the data for a subscription claim.
/// </summary>
public class SubscriptionClaimData
{
    /// <summary>
    /// Gets the trial status of the subscription.
    /// </summary>
    public bool Trial { get; private set; }

    /// <summary>
    /// Gets the name of the subscription plan.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the status of the subscription.
    /// </summary>
    public string Status { get; private set; }

    /// <summary>
    /// Gets the expiry date of the subscription in Unix time seconds.
    /// </summary>
    public string Expiry { get; private set; }

    /// <summary>
    /// The ID of an authenticated device.
    /// </summary>
    public string? DeviceId { get; private set; }

    //------------------------------------//

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionClaimData"/> class.
    /// </summary>
    /// <param name="sub">The team subscription.</param>
    public static SubscriptionClaimData Create(TeamSubscription sub, string? currentDvcUniqueId)
    {
        var data = new SubscriptionClaimData
        {
            Trial = sub.TrialEndDate > DateTime.Now,
            Name = sub.SubscriptionPlan!.Name,
            Status = sub.SubscriptionStatus.ToString().ToLower(),
            Expiry = new DateTimeOffset(sub.EndDate ?? DateTime.MaxValue).ToUnixTimeSeconds().ToString().ToLower()
        };
        if (currentDvcUniqueId is not null && sub.Devices.Any(dvc => dvc.UniqueId == currentDvcUniqueId))
            data.DeviceId = sub.Devices.First().Id.ToString();
        //Using Id rather than UniqueId. Incase the same Device is registered with multiple Subscriptions.
        //This way we can identify the correct Subscription.
        //DeviceAuthenticate is true for a Subscription if it has a non-null DeviceId

        return data;
    }

    //------------------------------------//

    #region Serializer CTOR
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    internal protected SubscriptionClaimData() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

}//Cls

