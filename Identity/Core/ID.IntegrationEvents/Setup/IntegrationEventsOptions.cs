namespace ID.IntegrationEvents.Setup;
public class IntegrationEventsOptions
{
    public EventBusProvider Provider { get; set; } = EventBusProvider.MassTransit;
    public bool UseInMemory { get; set; } = true;
    public string? ConnectionString { get; set; }
    public string? QueuePrefix { get; set; } = "myid";
    public TimeSpan? MessageTimeout { get; set; }

    /// <summary>
    /// When true, MyId will use a separate IMyIdMtBus to avoid conflicts.
    /// When false (default), MyId will use the standard IBus.
    /// </summary>
    public bool UseSeperateEventBus { get; set; } = false;
}