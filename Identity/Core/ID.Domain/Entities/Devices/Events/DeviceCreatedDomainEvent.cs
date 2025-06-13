using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.Teams;

namespace ID.Domain.Entities.Devices.Events;
public sealed record DeviceCreatedDomainEvent(Guid DeviceId, TeamDevice Device) : IIdDomainEvent { }
