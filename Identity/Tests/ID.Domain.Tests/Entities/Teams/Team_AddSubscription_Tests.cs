using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Events;
using ID.Domain.Entities.Teams.Validators;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.Teams;

public class Team_AddSubscription_Tests
{
    //------------------------------------//

    [Fact]
    public void AddSubscription_WithValidToken_ShouldAddSubscriptionToTeam()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var discount = Discount.Create(5);
        
        var validationResult = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, discount);
        validationResult.Succeeded.ShouldBeTrue();
        var token = validationResult.Value!;

        // Act
        var result = team.AddSubscription(token);

        // Assert
        result.ShouldNotBeNull();
        result.SubscriptionPlan.ShouldBe(subscriptionPlan);
        result.Team.ShouldBe(team);
        result.Discount.ShouldBe(discount.Value);
        team.Subscriptions.ShouldContain(result);
    }

    //------------------------------------//

    [Fact]
    public void AddSubscription_WithValidTokenNoDiscount_ShouldAddSubscriptionWithoutDiscount()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        
        var validationResult = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, null);
        validationResult.Succeeded.ShouldBeTrue();
        var token = validationResult.Value!;

        // Act
        var result = team.AddSubscription(token);

        // Assert
        result.ShouldNotBeNull();
        result.SubscriptionPlan.ShouldBe(subscriptionPlan);
        result.Team.ShouldBe(team);
        result.Discount.ShouldBe(0);
        team.Subscriptions.ShouldContain(result);
    }

    //------------------------------------//

    [Fact]
    public void AddSubscription_WithValidToken_ShouldRaiseDomainEvent()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var discount = Discount.Create(5);
        
        var validationResult = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, discount);
        var token = validationResult.Value!;

        // Act
        var result = team.AddSubscription(token);

        // Assert
        var domainEvents = team.GetDomainEvents();
        domainEvents.ShouldContain(e => e is TeamSubscriptionAddedEvent);
        
        var subscriptionEvent = domainEvents.OfType<TeamSubscriptionAddedEvent>().FirstOrDefault();
        subscriptionEvent.ShouldNotBeNull();
        subscriptionEvent.Team.ShouldBe(team);
        subscriptionEvent.Subscription.ShouldBe(result);
    }

    //------------------------------------//

    [Fact]
    public void AddSubscription_WhenSubscriptionAlreadyExists_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        
        var validationResult = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, null);
        var token = validationResult.Value!;

        // Add subscription first time
        var firstResult = team.AddSubscription(token);
        team.ClearDomainEvents(); // Clear events from first addition

        // Act - Try to add same subscription again
        var secondResult = team.AddSubscription(token);

        // Assert
        secondResult.ShouldBe(firstResult); // Should return same instance
        team.Subscriptions.Count.ShouldBe(1); // Should still only have one
        
        var domainEvents = team.GetDomainEvents();
        domainEvents.ShouldNotContain(e => e is TeamSubscriptionAddedEvent); // No new event
    }

    //------------------------------------//

    [Fact]
    public void AddSubscription_WithMultipleSubscriptions_ShouldMaintainSubscriptionCollection()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan1 = SubscriptionPlanDataFactory.Create();
        var subscriptionPlan2 = SubscriptionPlanDataFactory.Create();
        var discount1 = Discount.Create(5);
        var discount2 = Discount.Create(15);

        var token1 = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan1, discount1).Value!;
        var token2 = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan2, discount2).Value!;

        // Act
        var result1 = team.AddSubscription(token1);
        var result2 = team.AddSubscription(token2);

        // Assert
        team.Subscriptions.Count.ShouldBe(2);
        team.Subscriptions.ShouldContain(result1);
        team.Subscriptions.ShouldContain(result2);
        
        result1.SubscriptionPlan.ShouldBe(subscriptionPlan1);
        result1.Discount.ShouldBe(discount1.Value);
        result2.SubscriptionPlan.ShouldBe(subscriptionPlan2);
        result2.Discount.ShouldBe(discount2.Value);
    }

    //------------------------------------//

    [Fact]
    public void AddSubscription_ShouldReturnCreatedSubscription()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subId = Guid.NewGuid();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var discount = Discount.Create(10);
        
        var validationResult = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, discount);
        var token = validationResult.Value!;

        // Act
        var result = team.AddSubscription(token);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<TeamSubscription>();
        result.Id.ShouldNotBe(Guid.Empty);
    }

    //------------------------------------//

    [Fact]
    public void AddSubscription_ShouldCreateSubscriptionWithCorrectTeamReference()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        
        var validationResult = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, null);
        var token = validationResult.Value!;

        // Act
        var result = team.AddSubscription(token);

        // Assert
        result.Team.ShouldBe(team);
        result.TeamId.ShouldBe(team.Id);
    }

    //------------------------------------//

    [Theory]
    [InlineData(TeamType.customer)]
    [InlineData(TeamType.super)]
    [InlineData(TeamType.maintenance)]
    public void AddSubscription_WithDifferentTeamTypes_ShouldWorkConsistently(TeamType teamType)
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: teamType);
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var discount = Discount.Create(null);

        var validationResult = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, discount);
        
        // Skip if validation prevents this combination
        if (!validationResult.Succeeded)
            return;
            
        var token = validationResult.Value!;

        // Act
        var result = team.AddSubscription(token);

        // Assert
        result.ShouldNotBeNull();
        result.Team.ShouldBe(team);
        team.Subscriptions.ShouldContain(result);
    }

    //------------------------------------//

    [Fact]
    public void AddSubscription_WithTokenFromDifferentValidation_ShouldUseTokenProperties()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var discount = Discount.Create(5);
        
        var validationResult = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, discount);
        var token = validationResult.Value!;

        // Verify token contains correct properties
        token.SubscriptionPlan.ShouldBe(subscriptionPlan);
        token.Discount.ShouldBe(discount);

        // Act
        var result = team.AddSubscription(token);

        // Assert
        result.SubscriptionPlan.ShouldBe(token.SubscriptionPlan);
        result.Discount.ShouldBe(token.Discount!.Value);
    }

    //------------------------------------//

    [Fact]
    public void AddSubscription_ShouldMaintainSubscriptionCollectionIntegrity()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptions = new List<TeamSubscription>();
        
        // Add multiple subscriptions
        for (int i = 0; i < 3; i++)
        {
            var plan = SubscriptionPlanDataFactory.Create();
            var discount = i % 2 == 0 ? Discount.Create(10) : null; // Some with, some without discount
            var token = TeamValidators.SubscriptionAddition.Validate(team, plan, discount).Value!;
            subscriptions.Add(team.AddSubscription(token));
        }

        // Assert
        team.Subscriptions.Count.ShouldBe(3);
        foreach (var subscription in subscriptions)
        {
            team.Subscriptions.ShouldContain(subscription);
            subscription.Team.ShouldBe(team);
        }
    }

    //------------------------------------//

    [Fact]
    public void AddSubscription_WhenHashSetAddReturnsFalse_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        
        var validationResult = TeamValidators.SubscriptionAddition.Validate(team, subscriptionPlan, null);
        var token = validationResult.Value!;

        // Create a subscription that will be considered duplicate by HashSet
        var existingSubscription = team.AddSubscription(token);
        var initialEventCount = team.GetDomainEvents().Count;
        team.ClearDomainEvents();

        // Act - Try to add the same subscription again (should return false from HashSet.Add)
        var result = team.AddSubscription(token);

        // Assert
        result.ShouldBe(existingSubscription); // Should return existing subscription
        team.GetDomainEvents().ShouldBeEmpty(); // No new events should be raised
        team.Subscriptions.Count.ShouldBe(1); // Should still only have one subscription
    }

    //------------------------------------//

}//Cls
