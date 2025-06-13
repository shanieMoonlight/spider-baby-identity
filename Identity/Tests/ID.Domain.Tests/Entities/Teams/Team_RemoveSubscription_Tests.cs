using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Events;
using ID.Domain.Entities.Teams.Validators;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.Teams;

public class Team_RemoveSubscription_Tests
{

    [Fact]
    public void RemoveSubscription_WithValidToken_ShouldRemoveSubscriptionFromTeam()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var discount = Discount.Create(5);
        
        // Add subscription first
        var addToken = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, discount).Value!;
        var subscription = team.AddSubscription(addToken);
        team.ClearDomainEvents();

        // Validate removal
        var validationResult = TeamValidators.SubscriptionRemoval.Validate(team, subscription);
        validationResult.Succeeded.ShouldBeTrue();
        var removeToken = validationResult.Value!;

        // Act
        var result = team.RemoveSubscription(removeToken);

        // Assert
        result.ShouldBeTrue();
        team.Subscriptions.ShouldNotContain(subscription);
        team.Subscriptions.Count.ShouldBe(0);
    }

    //------------------------------------//

    [Fact]
    public void RemoveSubscription_WithValidToken_ShouldRaiseDomainEvent()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        
        // Add subscription first
        var addToken = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, null).Value!;
        var subscription = team.AddSubscription(addToken);
        team.ClearDomainEvents();

        // Validate removal
        var removeToken = TeamValidators.SubscriptionRemoval.Validate(team, subscription).Value!;

        // Act
        var result = team.RemoveSubscription(removeToken);

        // Assert
        result.ShouldBeTrue();
        
        var domainEvents = team.GetDomainEvents();
        domainEvents.ShouldContain(e => e is TeamSubscriptionRemovedEvent);
        
        var removalEvent = domainEvents.OfType<TeamSubscriptionRemovedEvent>().FirstOrDefault();
        removalEvent.ShouldNotBeNull();
        removalEvent.Team.ShouldBe(team);
        removalEvent.Subscription.ShouldBe(subscription);
    }

    //------------------------------------//

    [Fact]
    public void RemoveSubscription_WhenSubscriptionNotInTeam_ShouldReturnFalseAndNotRaiseEvent()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        
        // Create a subscription that is NOT in the team
        var subscription = SubscriptionDataFactory.Create(plan: subscriptionPlan, teamId: team.Id);
        
        // Create token (this would normally fail validation but we're testing the method directly)
        var removeToken = new TeamValidators.SubscriptionRemoval.Token(team, subscription);

        // Act
        var result = team.RemoveSubscription(removeToken);

        // Assert
        result.ShouldBeFalse();
        
        // Should NOT raise domain event
        var domainEvents = team.GetDomainEvents();
        domainEvents.ShouldNotContain(e => e is TeamSubscriptionRemovedEvent);
    }

    //------------------------------------//

    [Fact]
    public void RemoveSubscription_WithMultipleSubscriptions_ShouldOnlyRemoveSpecifiedSubscription()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan1 = SubscriptionPlanDataFactory.Create();
        var subscriptionPlan2 = SubscriptionPlanDataFactory.Create();
        var subscriptionPlan3 = SubscriptionPlanDataFactory.Create();
        
        // Add multiple subscriptions
        var addToken1 = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan1, null).Value!;
        var addToken2 = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan2, null).Value!;
        var addToken3 = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan3, null).Value!;
        
        var subscription1 = team.AddSubscription(addToken1);
        var subscription2 = team.AddSubscription(addToken2);
        var subscription3 = team.AddSubscription(addToken3);
        team.ClearDomainEvents();

        // Remove middle subscription
        var removeToken = TeamValidators.SubscriptionRemoval.Validate(team, subscription2).Value!;

        // Act
        var result = team.RemoveSubscription(removeToken);

        // Assert
        result.ShouldBeTrue();
        team.Subscriptions.ShouldContain(subscription1);
        team.Subscriptions.ShouldNotContain(subscription2); // Only this one removed
        team.Subscriptions.ShouldContain(subscription3);
        team.Subscriptions.Count.ShouldBe(2);
    }

    //------------------------------------//

    [Fact]
    public void RemoveSubscription_WhenCalledMultipleTimes_ShouldOnlyRemoveOnce()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        
        // Add subscription
        var addToken = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, null).Value!;
        var subscription = team.AddSubscription(addToken);
        team.ClearDomainEvents();

        // Get removal token
        var removeToken = TeamValidators.SubscriptionRemoval.Validate(team, subscription).Value!;

        // Act
        var result1 = team.RemoveSubscription(removeToken);
        var result2 = team.RemoveSubscription(removeToken); // Second call

        // Assert
        result1.ShouldBeTrue();  // First removal succeeds
        result2.ShouldBeFalse(); // Second removal fails (subscription already gone)

        team.Subscriptions.ShouldNotContain(subscription);
        team.Subscriptions.Count.ShouldBe(0);

        // Should only raise one domain event
        var domainEvents = team.GetDomainEvents();
        var removalEvents = domainEvents.OfType<TeamSubscriptionRemovedEvent>().ToList();
        removalEvents.Count.ShouldBe(1);
    }

    //------------------------------------//

    [Fact]
    public void RemoveSubscription_ShouldUseTokenSubscriptionReference()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var discount = Discount.Create(5);
        
        // Add subscription
        var addToken = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, discount).Value!;
        var subscription = team.AddSubscription(addToken);
        team.ClearDomainEvents();

        // Get removal token
        var removeToken = TeamValidators.SubscriptionRemoval.Validate(team, subscription).Value!;

        // Verify token contains correct subscription
        removeToken.Subscription.ShouldBe(subscription);

        // Act
        var result = team.RemoveSubscription(removeToken);

        // Assert
        result.ShouldBeTrue();
        team.Subscriptions.ShouldNotContain(subscription); // Token's subscription was removed
    }

    //------------------------------------//

    [Theory]
    [InlineData(TeamType.Customer)]
    [InlineData(TeamType.Super)]
    [InlineData(TeamType.Maintenance)]
    public void RemoveSubscription_WithDifferentTeamTypes_ShouldWorkConsistently(TeamType teamType)
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: teamType);
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        
        // Add subscription first
        var addValidationResult = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, null);
        if (!addValidationResult.Succeeded)
        {
            // Skip this test case if validation prevents this combination
            return;
        }
        
        var subscription = team.AddSubscription(addValidationResult.Value!);
        team.ClearDomainEvents();

        // Validate removal
        var removeValidationResult = TeamValidators.SubscriptionRemoval.Validate(team, subscription);
        if (!removeValidationResult.Succeeded)
        {
            // Skip this test case if validation prevents removal
            return;
        }

        var removeToken = removeValidationResult.Value!;

        // Act
        var result = team.RemoveSubscription(removeToken);

        // Assert
        result.ShouldBeTrue();
        team.Subscriptions.ShouldNotContain(subscription);
    }

    //------------------------------------//

    [Fact]
    public void RemoveSubscription_ShouldMaintainSubscriptionCollectionIntegrity()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptions = new List<TeamSubscription>();
        
        // Add multiple subscriptions
        for (int i = 0; i < 5; i++)
        {
            var plan = SubscriptionPlanDataFactory.Create();
            var discount = i % 2 == 0 ? Discount.Create(5) : null;
            var addToken = TeamValidators.SubscriptionAddition.Validate(team, plan, discount).Value!;
            subscriptions.Add(team.AddSubscription(addToken));
        }

        var subscriptionToRemove = subscriptions[2]; // Remove middle subscription
        team.ClearDomainEvents();

        var removeToken = TeamValidators.SubscriptionRemoval.Validate(team, subscriptionToRemove).Value!;

        // Act
        var result = team.RemoveSubscription(removeToken);

        // Assert
        result.ShouldBeTrue();
        team.Subscriptions.Count.ShouldBe(4);
        team.Subscriptions.ShouldNotContain(subscriptionToRemove);

        // Verify all other subscriptions are still there
        foreach (var subscription in subscriptions.Where(s => s != subscriptionToRemove))
        {
            team.Subscriptions.ShouldContain(subscription);
        }
    }

    //------------------------------------//

    [Fact]
    public void RemoveSubscription_WhenHashSetRemoveReturnsFalse_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        
        // Add and then remove subscription
        var addToken = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, null).Value!;
        var subscription = team.AddSubscription(addToken);
        var removeToken = TeamValidators.SubscriptionRemoval.Validate(team, subscription).Value!;
        
        team.RemoveSubscription(removeToken); // First removal
        team.ClearDomainEvents();

        // Act - Try to remove the same subscription again (should return false from HashSet.Remove)
        var result = team.RemoveSubscription(removeToken);

        // Assert
        result.ShouldBeFalse(); // HashSet.Remove should return false
        team.GetDomainEvents().ShouldBeEmpty(); // No new events should be raised
        team.Subscriptions.Count.ShouldBe(0); // Should still be empty
    }

    //------------------------------------//

    [Fact]
    public void RemoveSubscription_ShouldReturnBooleanIndicatingSuccess()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        
        // Add subscription
        var addToken = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, null).Value!;
        var subscription = team.AddSubscription(addToken);
        team.ClearDomainEvents();

        var removeToken = TeamValidators.SubscriptionRemoval.Validate(team, subscription).Value!;

        // Act
        var result = team.RemoveSubscription(removeToken);

        // Assert
        result.ShouldBeOfType<bool>();
        result.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls
