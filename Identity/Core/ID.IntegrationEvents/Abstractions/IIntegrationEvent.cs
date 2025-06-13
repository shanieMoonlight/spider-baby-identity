using MediatR;

namespace ID.IntegrationEvents.Abstractions;

/// <summary>
/// Requires a paramaterless ctor
/// </summary>
public interface IIdIntegrationEvent : INotification { }

/// <summary>
/// Represents the base class for integration events in the system.
/// </summary>
/// <remarks>Integration events are used to facilitate communication between different parts of the system or 
/// across services. This abstract class provides a common structure for all integration events,  including a timestamp
/// indicating when the event occurred.</remarks>
public abstract record AIdIntegrationEvent : IIdIntegrationEvent
{
    public DateTime OccurredAt { get; protected set; } = DateTime.UtcNow;
    public Guid EventId { get; protected set; } = Guid.NewGuid();
    public string EventVersion { get; protected set; } = "1.0";
    
}
