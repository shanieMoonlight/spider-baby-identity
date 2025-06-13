using MassTransit;
using MediatR;

namespace MyIdDemo.Setup.Events.MT.Abstractions;

public abstract class AMyIdDemoEventHandler<T>
    : IConsumer<T>, INotificationHandler<T>
    where T : class, INotification
{
    /// <summary>
    /// Internal use only. Do not call or override
    /// </summary>
    public async Task Consume(ConsumeContext<T> context) =>
        await HandleEventAsync(context.Message);

    /// <summary>
    /// Internal use only. Do not call or override
    /// </summary>
    public async Task Handle(T notification, CancellationToken cancellationToken) =>
        await HandleEventAsync(notification);


    public abstract Task HandleEventAsync(T notification);

}
