using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.Teams;

namespace ID.Domain.Entities.Devices.Events;
public sealed record DeviceUpdatedDomainEvent(Guid DeviceId, TeamDevice Device) : IIdDomainEvent
{
    
}
