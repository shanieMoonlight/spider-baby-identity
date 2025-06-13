using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.IntegrationEvents.Abstractions;

namespace ID.IntegrationEvents.Events.Account.PhoneConfirmation;

//#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-//

public record PhoneConfirmationIntegrationEvent : AIdIntegrationEvent
{
    public Guid UserId { get; init; }
    public string Phone { get; init; }
    public string Username { get; init; }
    public string ConfirmationToken { get; init; }
    public bool IsCustomerTeam { get; init; }

    //------------------------//

    #region MassTransitCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Required for MassTransit. Do not use.
    /// </summary>
    public PhoneConfirmationIntegrationEvent() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    //- - - - - - - - - - - - - - - - - - -//

    public PhoneConfirmationIntegrationEvent(AppUser user, string confirmationToken, TeamType type)
    {
        UserId = user.Id;
        Phone = user.PhoneNumber!; //Let the consumer handle it.
        ConfirmationToken = confirmationToken;
        IsCustomerTeam = type == TeamType.Customer;


        Username = user.FirstName;
        if (string.IsNullOrWhiteSpace(Username))
            Username = user.UserName ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Username))
            Username = user.Email ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Username))
            Username = "User";
    }

}//Cls