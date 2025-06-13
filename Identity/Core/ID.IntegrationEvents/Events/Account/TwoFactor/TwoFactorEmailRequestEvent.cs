using ID.Domain.Entities.AppUsers;
using ID.IntegrationEvents.Abstractions;

namespace ID.IntegrationEvents.Events.Account.TwoFactor;

public record TwoFactorEmailRequestIntegrationEvent : AIdIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string Name { get; set; }
    public string VerificationCode { get; set; }

    //------------------------//

    #region MassTransitCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Required for MassTransit. Do not use.
    /// </summary>
    public TwoFactorEmailRequestIntegrationEvent() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    //- - - - - - - - - - - - - - - - - - //

    public TwoFactorEmailRequestIntegrationEvent(
        Guid userId,
        string email,
        string? phone,
        string name,
        string verificationCode)
    {
        Email = email;
        Phone = phone;
        Name = name;
        VerificationCode = verificationCode;
        UserId = userId;
    }


    public TwoFactorEmailRequestIntegrationEvent(AppUser user, string verificationCode)
    {
        Email = user.Email ?? string.Empty; //Let the consumer handle it.
        Phone = user.PhoneNumber;
        Name = user.FirstName ?? user.UserName ?? "User";
        VerificationCode = verificationCode;
        UserId = user.Id;
    }

    //------------------------//

}//Cls
