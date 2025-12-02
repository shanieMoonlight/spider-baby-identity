using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Devices.Events;
public sealed record DeviceUpdatedDomainEvent(Guid DeviceId) : IIdDomainEvent;