using ID.IntegrationEvents.Abstractions;

namespace ID.IntegrationEvents.Events.Teams;

public record TeamLeaderChangedEvent : AIdIntegrationEvent
{
    public Guid TeamId { get; set; }

    //------------------------//

    #region MassTransitCtor
    /// <summary>
    /// Required for MassTransit. Do not use.
    /// </summary>
    public TeamLeaderChangedEvent() { }
    #endregion

    //- - - - - - - - - - - - - - - - - - //

    public TeamLeaderChangedEvent(Guid teamId) => 
        TeamId = teamId;

    //------------------------//
}
