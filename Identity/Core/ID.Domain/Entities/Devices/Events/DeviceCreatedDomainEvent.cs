using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Devices.Events;
public sealed record DeviceCreatedDomainEvent(Guid DeviceId) : IIdDomainEvent { }
