namespace ID.IntegrationEvents.Abstractions;

public interface IEventBus
{
    Task Publish<T>(T message, CancellationToken cancellationToken = default)
        where T : class, IIdIntegrationEvent;
}