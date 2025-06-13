using ID.Domain.Abstractions.Events;
using System.ComponentModel.DataAnnotations;

namespace ID.Domain.Entities.Common;
public abstract class IdDomainEntity : IIdDomainEventEntity, IIdAuditableDomainEntity
{
    public Guid Id { get; internal set; }

    [MaxLength(100)]
    public string? AdministratorUsername { get; protected set; }
    [MaxLength(100)]
    public string? AdministratorId { get; private set; }

    public DateTime DateCreated { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }

    //------------------------//

    protected IdDomainEntity(Guid id) => Id = id;

    protected IdDomainEntity() { }

    //------------------------//

    public IIdAuditableDomainEntity SetModified(string? username, string? userId)
    {
        LastModifiedDate = DateTime.UtcNow;
        AdministratorUsername = username;
        AdministratorId = userId;
        return this;
    }

    //------------------------//

    public IIdAuditableDomainEntity SetCreated(string? username, string? userId)
    {
        DateCreated = DateTime.UtcNow;
        AdministratorUsername = username;
        AdministratorId = userId;
        return this;
    }

    //------------------------//


    protected readonly List<IIdDomainEvent> _domainEvents = [];

    //- - - - - - - - - - - - //

    protected void RaiseDomainEvent(IIdDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    //- - - - - - - - - - - - //

    public void ClearDomainEvents() => _domainEvents.Clear();

    //- - - - - - - - - - - - //

    public IReadOnlyList<IIdDomainEvent> GetDomainEvents() => [.. _domainEvents];

    //------------------------//

}//Cls
