using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.TrustedDevices;
using ID.IntegrationEvents.Abstractions;

namespace ID.IntegrationEvents.Events.Account.TrustedDevices;

public record TrustedDeviceRevokedIntegrationEvent : AIdIntegrationEvent
{
    public Guid DeviceId { get; set; }
    public string UserEmail { get; set; }
    public string UserName { get; set; }
    public string? Phone { get; set; }
    public string UserAgent { get; set; }
    public string IpAddress { get; set; }
    public string DeviceName { get; set; }
    public bool IsCustomerTeam { get; set; }

    public DateTime DateRevoked { get; set; } = DateTime.UtcNow;

    //------------------------//

    #region MassTransitCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Required for MassTransit. Do not use.
    /// </summary>
    public TrustedDeviceRevokedIntegrationEvent() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    //- - - - - - - - - - - - //

    public TrustedDeviceRevokedIntegrationEvent(TrustedDevice device, AppUser user, Team team)
    {
        UserEmail = user.Email ?? string.Empty; //Let the consumer handle it.  
        Phone = user.PhoneNumber;
        UserName = user.FirstName ?? user.UserName ?? "User";
        DeviceName = device.Name ?? device.Fingerprint;
        DeviceId = device.Id;
        UserAgent = device.UserAgent;
        IpAddress = device.IpAddress;
        DateRevoked = device.LastModifiedDate ?? DateTime.UtcNow;
        IsCustomerTeam = team.IsCustomerTeam;
    }


}//Cls
