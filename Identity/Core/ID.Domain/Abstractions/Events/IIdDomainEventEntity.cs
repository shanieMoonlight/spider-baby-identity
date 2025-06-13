using MediatR;

namespace ID.Domain.Abstractions.Events;

public interface IIdDomainEventEntity : INotification
{
    IReadOnlyList<IIdDomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}
