using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Validators;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.SubPlans;
using MyResults;

namespace ID.Infrastructure.DomainServices.Teams.Subs;
internal class SubscriptionService(IIdUnitOfWork uow, Team team) : ITeamSubscriptionService
{
    private readonly IIdentityTeamRepo _teamRepo = uow.TeamRepo;
    private readonly IIdentitySubscriptionPlanRepo _subPlanRepo = uow.SubscriptionPlanRepo;
    public Team Team { get; } = team;

    //-----------------------//

    /// <summary>
    /// Retrieves a subscription by its ID.
    /// </summary>
    /// <param name="subId">The ID of the subscription to retrieve.</param>
    /// <returns>The subscription if found; otherwise, null.</returns>
    public Task<TeamSubscription?> GetSubscriptionAsync(Guid? subId) =>
        Task.FromResult(Team.Subscriptions.FirstOrDefault(s => s.Id == subId));

    //-----------------------//

    /// <summary>
    /// Adds a subscription to the team using the specified plan ID and discount.
    /// </summary>
    /// <param name="planId">The ID of the subscription plan.</param>
    /// <param name="discount">The discount to apply to the subscription.</param>
    /// <returns>A result containing the added subscription or a not found result if the plan does not exist.</returns>
    public async Task<GenResult<TeamSubscription>> AddSubscriptionAsync(Guid? planId, Discount? discount)
    {
        var plan = await _subPlanRepo.FirstOrDefaultAsync(new SubPlanByIdWithFlagsSpec(planId));

        if (plan is null)
            return GenResult<TeamSubscription>.NotFoundResult(IDMsgs.Error.NotFound<SubscriptionPlan>(planId));

        return await AddSubscriptionAsync(plan, discount);
    }

    //- - - - - - - - - - - -//

    /// <summary>
    /// Adds a subscription to the team using the specified plan and discount.
    /// </summary>
    /// <param name="plan">The subscription plan to use.</param>
    /// <param name="discount">The discount to apply to the subscription.</param>
    /// <returns>The added subscription.</returns>
    public async Task<GenResult<TeamSubscription>> AddSubscriptionAsync(SubscriptionPlan plan, Discount? discount)
    {
        var validationResult = TeamValidators.SubscriptionAddition.Validate(Team, plan, discount);
        if (!validationResult.Succeeded)
            return validationResult.Convert<TeamSubscription>();

        var validationToken = validationResult.Value!; // Success is non-null
        var team = Team.AddSubscription(validationToken);

        await UpdateAndSaveAsync();
        return GenResult<TeamSubscription>.Success(team);
    }

    //-----------------------//

    /// <summary>
    /// Removes a subscription from the team by its ID.
    /// </summary>
    /// <param name="subId">The ID of the subscription to remove.</param>
    /// <returns>A result indicating whether the removal was successful or a not found result if the subscription does not exist.</returns>
    public async Task<GenResult<bool>> RemoveSubscriptionAsync(Guid? subId)
    {
        var sub = Team.Subscriptions.FirstOrDefault(s => s.Id == subId);

        if (sub is null)
            return GenResult<bool>.NotFoundResult(IDMsgs.Error.NotFound<TeamSubscription>(subId));

        return await RemoveSubscriptionAsync(sub);
    }

    //-----------------------//

    /// <summary>
    /// Removes a subscription from the team.
    /// </summary>
    /// <param name="sub">The subscription to remove.</param>
    /// <returns>True if the subscription was removed; otherwise, false.</returns>
    public async Task<GenResult<bool>> RemoveSubscriptionAsync(TeamSubscription sub)
    {
        var validationResult = TeamValidators.SubscriptionRemoval.Validate(Team, sub);
        if (!validationResult.Succeeded)
            return validationResult.Convert<bool>();

        var validationToken = validationResult.Value!; // Success is non-null

        var removed = Team.RemoveSubscription(validationToken);
        if (removed) //Not already moved
            await UpdateAndSaveAsync();

        return GenResult<bool>.Success(removed);
    }

    //-----------------------//

    /// <summary>
    /// Records a payment for a subscription by its ID.
    /// </summary>
    /// <param name="subId">The ID of the subscription to record the payment for.</param>
    /// <returns>A result containing the subscription or a not found result if the subscription does not exist.</returns>
    public async Task<GenResult<TeamSubscription>> RecordPaymentAsync(Guid? subId)
    {
        var sub = Team.Subscriptions.FirstOrDefault(s => s.Id == subId);

        if (sub is null)
            return GenResult<TeamSubscription>.NotFoundResult(IDMsgs.Error.NotFound<TeamSubscription>(subId));

        await RecordPaymentAsync(sub);
        return GenResult<TeamSubscription>.Success(sub);
    }

    //-----------------------//

    /// <summary>
    /// Records a payment for a subscription.
    /// </summary>
    /// <param name="sub">The subscription to record the payment for.</param>
    /// <returns>The subscription with the recorded payment.</returns>
    public async Task<TeamSubscription> RecordPaymentAsync(TeamSubscription sub)
    {
        sub.RecordPayment();
        await UpdateAndSaveAsync();
        return sub;
    }

    //-----------------------//

    /// <summary>
    /// Updates the team and saves the changes.
    /// </summary>
    /// <returns>The updated team.</returns>
    public async Task<Team> UpdateAsync() =>
        await UpdateAndSaveAsync();

    //-----------------------//

    /// <summary>
    /// Updates the team and saves the changes.
    /// </summary>
    /// <returns>The updated team.</returns>
    private async Task<Team> UpdateAndSaveAsync()
    {
        var updatedTeam = await _teamRepo.UpdateAsync(Team);
        await uow.SaveChangesAsync();
        return updatedTeam;
    }

    //-----------------------//

}//Cls
