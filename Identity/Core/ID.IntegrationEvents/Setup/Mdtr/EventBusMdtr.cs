using ID.IntegrationEvents.Abstractions;
using MediatR;

namespace ID.IntegrationEvents.Setup.Mdtr;
internal class EventBusMdtr(IPublisher bus) : IEventBus
{
    public async Task Publish<T>(T message, CancellationToken cancellationToken) where T : class, IIdIntegrationEvent =>
        await bus.Publish(message, cancellationToken);
}
