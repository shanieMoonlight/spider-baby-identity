using ID.Domain.Entities.SubscriptionPlans.Events;

namespace ID.Domain.Tests.Entities.SubscriptionPlan;

public class SubscriptionPlan_DomainEvents_Tests
{
    //------------------------------------//

    [Fact]
    public void AddFeatureFlag_WhenFlagIsNew_ShouldRaiseDomainEvent()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var featureFlag = FeatureFlagDataFactory.Create();

        subscriptionPlan.ClearDomainEvents(); // Ensure clean state

        // Act
        subscriptionPlan.AddFeatureFlag(featureFlag);

        // Assert
        var domainEvents = subscriptionPlan.GetDomainEvents();
        domainEvents.ShouldHaveSingleItem();

        var domainEvent = domainEvents[0].ShouldBeOfType<SubscriptionPlanFeatureAddedDomainEvent>();
        domainEvent.SubscriptionPlanId.ShouldBe(subscriptionPlan.Id);
        domainEvent.SubscriptionPlanId.ShouldBe(subscriptionPlan.Id);
        domainEvent.FeatureFlagId.ShouldBe(featureFlag.Id   );
    }

    //------------------------------------//

    [Fact]
    public void AddFeatureFlag_WhenFlagAlreadyExists_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var featureFlag = FeatureFlagDataFactory.Create();

        // Add flag first time
        subscriptionPlan.AddFeatureFlag(featureFlag);
        subscriptionPlan.ClearDomainEvents(); // Clear events from first add

        // Act - Try to add same flag again
        subscriptionPlan.AddFeatureFlag(featureFlag);

        // Assert
        var domainEvents = subscriptionPlan.GetDomainEvents();
        domainEvents.ShouldBeEmpty();
    }

    //------------------------------------//

    [Fact]
    public void AddFeatureFlags_WithMultipleNewFlags_ShouldRaiseMultipleDomainEvents()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var featureFlags = new[]
        {
            FeatureFlagDataFactory.Create(),
            FeatureFlagDataFactory.Create(),
            FeatureFlagDataFactory.Create()
        };

        subscriptionPlan.ClearDomainEvents();

        // Act
        subscriptionPlan.AddFeatureFlags(featureFlags);

        // Assert
        var domainEvents = subscriptionPlan.GetDomainEvents();
        domainEvents.Count.ShouldBe(3);

        foreach (var domainEvent in domainEvents)
        {
            var featureAddedEvent = domainEvent.ShouldBeOfType<SubscriptionPlanFeatureAddedDomainEvent>();
            featureAddedEvent.SubscriptionPlanId.ShouldBe(subscriptionPlan.Id);
            featureFlags.ShouldContain(f => f.Id == featureAddedEvent.FeatureFlagId);
        }
    }

    //------------------------------------//

    [Fact]
    public void AddFeatureFlags_WithMixOfNewAndExistingFlags_ShouldOnlyRaiseEventsForNewFlags()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var existingFlag = FeatureFlagDataFactory.Create();
        var newFlag1 = FeatureFlagDataFactory.Create();
        var newFlag2 = FeatureFlagDataFactory.Create();

        // Add existing flag first
        subscriptionPlan.AddFeatureFlag(existingFlag);
        subscriptionPlan.ClearDomainEvents();

        var flagsToAdd = new[] { existingFlag, newFlag1, newFlag2 };

        // Act
        subscriptionPlan.AddFeatureFlags(flagsToAdd);

        // Assert
        var domainEvents = subscriptionPlan.GetDomainEvents();
        domainEvents.Count.ShouldBe(2); // Only for new flags

        var raisedFlagIds = domainEvents
            .Cast<SubscriptionPlanFeatureAddedDomainEvent>()
            .Select(e => e.FeatureFlagId)
            .ToList();

        raisedFlagIds.ShouldContain(newFlag1.Id);
        raisedFlagIds.ShouldContain(newFlag2.Id );
        raisedFlagIds.ShouldNotContain(existingFlag.Id);
    }

    //------------------------------------//

    [Fact]
    public void RemoveFeatureFlag_WhenFlagExists_ShouldRaiseDomainEvent()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var featureFlag = FeatureFlagDataFactory.Create();

        // Add flag first
        subscriptionPlan.AddFeatureFlag(featureFlag);
        subscriptionPlan.ClearDomainEvents(); // Clear add event

        // Act
        subscriptionPlan.RemoveFeatureFlag(featureFlag);

        // Assert
        var domainEvents = subscriptionPlan.GetDomainEvents();
        domainEvents.ShouldHaveSingleItem();

        var domainEvent = domainEvents[0].ShouldBeOfType<SubscriptionPlanFeatureRemovedDomainEvent>();
        domainEvent.SubscriptionPlanId.ShouldBe(subscriptionPlan.Id);
        domainEvent.FeatureFlagId.ShouldBe(featureFlag.Id);
    }

    //------------------------------------//

    [Fact]
    public void RemoveFeatureFlag_WhenFlagDoesNotExist_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var featureFlag = FeatureFlagDataFactory.Create();

        subscriptionPlan.ClearDomainEvents();

        // Act - Try to remove flag that was never added
        subscriptionPlan.RemoveFeatureFlag(featureFlag);

        // Assert
        var domainEvents = subscriptionPlan.GetDomainEvents();
        domainEvents.ShouldBeEmpty();
    }

    //------------------------------------//

    [Fact]
    public void RemoveFeatureFlags_WithMultipleExistingFlags_ShouldRaiseMultipleDomainEvents()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var featureFlags = new[]
        {
            FeatureFlagDataFactory.Create(),
            FeatureFlagDataFactory.Create(),
            FeatureFlagDataFactory.Create()
        };

        // Add all flags first
        subscriptionPlan.AddFeatureFlags(featureFlags);
        subscriptionPlan.ClearDomainEvents(); // Clear add events

        // Act
        subscriptionPlan.RemoveFeatureFlags(featureFlags);

        // Assert
        var domainEvents = subscriptionPlan.GetDomainEvents();
        domainEvents.Count.ShouldBe(3);

        foreach (var domainEvent in domainEvents)
        {
            var featureRemovedEvent = domainEvent.ShouldBeOfType<SubscriptionPlanFeatureRemovedDomainEvent>();
            featureRemovedEvent.SubscriptionPlanId.ShouldBe(subscriptionPlan.Id);
            featureFlags.ShouldContain(f => f.Id == featureRemovedEvent.FeatureFlagId);
        }
    }

    //------------------------------------//

    [Fact]
    public void RemoveFeatureFlags_WithMixOfExistingAndNonExistingFlags_ShouldOnlyRaiseEventsForExistingFlags()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var existingFlag1 = FeatureFlagDataFactory.Create();
        var existingFlag2 = FeatureFlagDataFactory.Create();
        var nonExistingFlag = FeatureFlagDataFactory.Create();

        // Add only some flags
        subscriptionPlan.AddFeatureFlags([existingFlag1, existingFlag2]);
        subscriptionPlan.ClearDomainEvents();

        var flagsToRemove = new[] { existingFlag1, existingFlag2, nonExistingFlag };

        // Act
        subscriptionPlan.RemoveFeatureFlags(flagsToRemove);

        // Assert
        var domainEvents = subscriptionPlan.GetDomainEvents();
        domainEvents.Count.ShouldBe(2); // Only for existing flags

        var removedFlagIds = domainEvents
            .Cast<SubscriptionPlanFeatureRemovedDomainEvent>()
            .Select(e => e.FeatureFlagId)
            .ToList();

        removedFlagIds.ShouldContain(existingFlag1.Id);
        removedFlagIds.ShouldContain(existingFlag2.Id);
        removedFlagIds.ShouldNotContain(nonExistingFlag.Id);
    }

    //------------------------------------//

    [Fact]
    public void AddAndRemoveFeatureFlag_ShouldRaiseBothDomainEvents()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var featureFlag = FeatureFlagDataFactory.Create();

        subscriptionPlan.ClearDomainEvents();

        // Act
        subscriptionPlan.AddFeatureFlag(featureFlag);
        subscriptionPlan.RemoveFeatureFlag(featureFlag);

        // Assert
        var domainEvents = subscriptionPlan.GetDomainEvents();
        domainEvents.Count.ShouldBe(2);

        var addEvent = domainEvents[0].ShouldBeOfType<SubscriptionPlanFeatureAddedDomainEvent>();
        var removeEvent = domainEvents[domainEvents.Count - 1].ShouldBeOfType<SubscriptionPlanFeatureRemovedDomainEvent>();

        addEvent.FeatureFlagId.ShouldBe(featureFlag.Id);
        removeEvent.FeatureFlagId.ShouldBe(featureFlag.Id   );
    }

    //------------------------------------//

    [Fact]
    public void AddFeatureFlag_ShouldMaintainFeatureFlagCollection()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var featureFlag = FeatureFlagDataFactory.Create();

        // Act
        subscriptionPlan.AddFeatureFlag(featureFlag);

        // Assert
        subscriptionPlan.FeatureFlags.ShouldContain(featureFlag);
        subscriptionPlan.FeatureFlags.Count.ShouldBe(1);
    }

    //------------------------------------//

    [Fact]
    public void RemoveFeatureFlag_ShouldMaintainFeatureFlagCollection()
    {
        // Arrange
        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var featureFlag = FeatureFlagDataFactory.Create();

        subscriptionPlan.AddFeatureFlag(featureFlag);

        // Act
        subscriptionPlan.RemoveFeatureFlag(featureFlag);

        // Assert
        subscriptionPlan.FeatureFlags.ShouldNotContain(featureFlag);
        subscriptionPlan.FeatureFlags.ShouldBeEmpty();
    }

    //------------------------------------//

}//Cls