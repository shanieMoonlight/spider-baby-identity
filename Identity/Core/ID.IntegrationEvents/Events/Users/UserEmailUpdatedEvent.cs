using ID.IntegrationEvents.Abstractions;

namespace ID.IntegrationEvents.Events.Users;

public record UserEmailUpdatedEvent : AIdIntegrationEvent
{
    public Guid UserId { get; set; }
    public Guid TeamId { get; set; }
    public string Email { get; set; }

    //------------------------//


    #region MassTransitCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Required for MassTransit. Do not use.
    /// </summary>
    public UserEmailUpdatedEvent() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    //- - - - - - - - - - - - - - - - - - //

    public UserEmailUpdatedEvent(Guid userId, Guid teamId, string email)
    {
        UserId = userId;
        TeamId = teamId;
        Email = email;
    }

    //------------------------//


}
