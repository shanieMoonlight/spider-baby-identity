using ID.Domain.Entities.AppUsers;
using ID.IntegrationEvents.Abstractions;

namespace ID.IntegrationEvents.Events.Account.TwoFactor;

public record TwoFactorGoogleSetupRequestIntegrationEvent : AIdIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string QrSrc { get; set; }
    public string Name { get; set; }
    public string ManualQrCode { get; set; }


    //------------------------//

    #region MassTransitCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Required for MassTransit. Do not use.
    /// </summary>
    public TwoFactorGoogleSetupRequestIntegrationEvent() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    //- - - - - - - - - - - - - - - - - - -//

    public TwoFactorGoogleSetupRequestIntegrationEvent(AppUser user, string qrSrc, string manualQrCode)
    {
        Email = user.Email ?? string.Empty; //Let the consumer handle it.  
        Name = user.FirstName ?? user.UserName ?? "User";
        UserId = user.Id;
        QrSrc = qrSrc;
        ManualQrCode = manualQrCode;
    }

    //------------------------//
};
