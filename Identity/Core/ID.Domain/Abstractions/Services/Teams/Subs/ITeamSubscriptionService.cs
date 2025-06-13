using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using MyResults;

namespace ID.Domain.Abstractions.Services.Teams.Subs;

/// <summary>
/// Access through ITeamSubsriptionServiceFactory
/// </summary>
public interface ITeamSubscriptionService
{
    /// <summary>
    /// Gets the team associated with the subscription service.
    /// </summary>
    Team Team { get; }

    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds a subscription to the team using the specified plan and discount.
    /// </summary>
    /// <param name="plan">The subscription plan to use.</param>
    /// <param name="discount">The discount to apply to the subscription.</param>
    /// <returns>The added subscription.</returns>
    Task<GenResult<TeamSubscription>> AddSubscriptionAsync(SubscriptionPlan plan, Discount? discount);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds a subscription to the team using the specified plan ID and discount.
    /// </summary>
    /// <param name="planId">The ID of the subscription plan.</param>
    /// <param name="discount">The discount to apply to the subscription.</param>
    /// <returns>A result containing the added subscription or a not found result if the plan does not exist.</returns>
    Task<GenResult<TeamSubscription>> AddSubscriptionAsync(Guid? planId, Discount? discount);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a subscription by its ID.
    /// </summary>
    /// <param name="subId">The ID of the subscription to retrieve.</param>
    /// <returns>The subscription if found; otherwise, null.</returns>
    Task<TeamSubscription?> GetSubscriptionAsync(Guid? subId);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Records a payment for a subscription by its ID.
    /// </summary>
    /// <param name="subId">The ID of the subscription to record the payment for.</param>
    /// <returns>A result containing the subscription or a not found result if the subscription does not exist.</returns>
    Task<GenResult<TeamSubscription>> RecordPaymentAsync(Guid? subId);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Records a payment for a subscription.
    /// </summary>
    /// <param name="sub">The subscription to record the payment for.</param>
    /// <returns>The subscription with the recorded payment.</returns>
    Task<TeamSubscription> RecordPaymentAsync(TeamSubscription sub);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Removes a subscription from the team by its ID.
    /// </summary>
    /// <param name="subId">The ID of the subscription to remove.</param>
    /// <returns>A result indicating whether the removal was successful or a not found result if the subscription does not exist.</returns>
    Task<GenResult<bool>> RemoveSubscriptionAsync(Guid? subId);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Removes a subscription from the team.
    /// </summary>
    /// <param name="sub">The subscription to remove.</param>
    /// <returns>True if the subscription was removed; otherwise, false.</returns>
    Task<GenResult<bool>> RemoveSubscriptionAsync(TeamSubscription sub);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Updates the team.
    /// </summary>
    /// <returns>The updated team.</returns>
    Task<Team> UpdateAsync();

}//Int
