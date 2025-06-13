namespace MyIdDemo.Setup.Events.MT.Abstractions;

public interface IMyIdDemoEventBus
{
    Task Publish<T>(T message, CancellationToken cancellationToken = default)
        where T : class, IMyIdDemoIntegrationEvent;
}