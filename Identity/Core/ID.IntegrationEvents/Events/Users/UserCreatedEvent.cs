using ID.Domain.Entities.Teams;
using ID.IntegrationEvents.Abstractions;

namespace ID.IntegrationEvents.Events.Users;

public record UserCreatedEvent : AIdIntegrationEvent
{
    public Guid UserId { get; set; }
    public Guid TeamId { get; set; }
    public int Position { get; set; }
    public TeamType TeamType { get; set; }

    //------------------------//

    #region MassTransitCtor
    /// <summary>
    /// Required for MassTransit. Do not use.
    /// </summary>
    public UserCreatedEvent() { }
    #endregion

    //- - - - - - - - - - - - - - - - - - //

    public UserCreatedEvent(Guid userId, Guid teamId, int position, TeamType teamType)
    {
        UserId = userId;
        TeamId = teamId;
        Position = position;
        TeamType = teamType;
    }

    //------------------------//

}//Cls
