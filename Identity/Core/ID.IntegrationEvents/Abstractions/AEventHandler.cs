using MassTransit;
using MediatR;

namespace ID.IntegrationEvents.Abstractions;

/// <summary>
/// Base class for event handlers
/// Will respond to both MassTransit and MediatR
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AEventHandler<T>
    : IConsumer<T>, INotificationHandler<T>
    where T : class, INotification
{
    //- - - - - - - - - - - - - //

    /// <summary>
    /// Internal use only. Do not call or override.
    /// This is used by MassTransit.
    /// </summary>
    public async Task Consume(ConsumeContext<T> context) =>
        await HandleEventAsync(context.Message);

    //- - - - - - - - - - - - - //

    /// <summary>
    /// Internal use only. Do not call or override.
    /// This is used by Mediatr
    /// </summary>
    public async Task Handle(T notification, CancellationToken cancellationToken) =>
        await HandleEventAsync(notification);

    //- - - - - - - - - - - - - //

    public abstract Task HandleEventAsync(T notification);


}//Cls
