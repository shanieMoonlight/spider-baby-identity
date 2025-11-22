using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.TrustedDevices;
using ID.IntegrationEvents.Abstractions;

namespace ID.IntegrationEvents.Events.Account.TrustedDevices;

public record TrustedDeviceAddedIntegrationEvent : AIdIntegrationEvent
{
    public Guid DeviceId { get; set; }
    public string UserEmail { get; set; }
    public string UserName { get; set; }
    public string? FirstName { get; set; }
    public string? Phone { get; set; }
    public string DeviceName { get; set; }

    public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;

    //------------------------//

    #region MassTransitCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Required for MassTransit. Do not use.
    /// </summary>
    public TrustedDeviceAddedIntegrationEvent() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    //- - - - - - - - - - - - //


    public TrustedDeviceAddedIntegrationEvent(AppUser user, TrustedDevice device)
    {
        UserEmail = user.Email ?? string.Empty; //Let the consumer handle it.  
        Phone = user.PhoneNumber;
        UserName = user.FirstName ?? user.UserName ?? "User";
        DeviceName = device.Name ?? device.DeviceFingerprint;
        DeviceId = device.Id;
    }


}//Cls
