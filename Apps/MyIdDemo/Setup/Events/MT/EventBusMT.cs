using MassTransit;
using MyIdDemo.Setup.Events.MT.Abstractions;
using System.Diagnostics;

namespace MyIdDemo.Setup.Events.MT;
internal class EventBusMT(IBus bus) : IMyIdDemoEventBus
{

    public async Task Publish<T>(T message, CancellationToken cancellationToken) where T : class, IMyIdDemoIntegrationEvent
    {
        await bus.Publish(message, cancellationToken);
    }
}
