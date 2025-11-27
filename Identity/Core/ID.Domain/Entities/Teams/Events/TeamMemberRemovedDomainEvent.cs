using ID.Domain.Abstractions.Events;

namespace ID.Domain.Entities.Teams.Events;
public sealed record TeamMemberRemovedDomainEvent(Guid TeamId, Guid MemberId) : IIdDomainEvent;