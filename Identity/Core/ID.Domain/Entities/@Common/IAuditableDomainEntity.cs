using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Common;
public interface IIdAuditableDomainEntity: IIdBaseDomainEntity
{
    string? AdministratorId { get; }
    string? AdministratorUsername { get; }
    DateTime DateCreated { get; }
    DateTime? LastModifiedDate { get; }

    void ClearDomainEvents();
    IReadOnlyList<IIdDomainEvent> GetDomainEvents();
    IIdAuditableDomainEntity SetCreated(string? username, string? userId);
    IIdAuditableDomainEntity SetModified(string? username, string? userId);
    //IIdAuditableDomainEntity TestSetCreated(string? username, string? userId, DateTime? date);
    //IIdAuditableDomainEntity TestSetModified(string? username, string? userId, DateTime? date);
}