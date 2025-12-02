using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.SubscriptionPlans.Subscriptions.Events;
public sealed record SubscriptionDeviceAddedDomainEvent(Guid SubscriptionId, Guid DeviceId) : IIdDomainEvent;