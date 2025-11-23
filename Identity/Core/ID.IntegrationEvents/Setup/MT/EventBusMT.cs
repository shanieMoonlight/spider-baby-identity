using ID.IntegrationEvents.Abstractions;
using MassTransit;

namespace ID.IntegrationEvents.Setup.MT;
internal class EventBusMT(IBus bus) : IEventBus
{

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken) 
        where T : class, IIdIntegrationEvent => 
        await bus.Publish(message, cancellationToken);
}
