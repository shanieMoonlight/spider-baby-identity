using ID.Domain.Entities.TrustedDevices;

namespace ID.Application.Features.Account.Cmd.TrustedDevices;
public class TrustedDeviceDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public string DeviceFingerprint { get; set; } 
    public string Name { get; set; } 
    public string? UserAgent { get; set; }
    public DateTime LastUsedDate { get; set; }
    public string? AdministratorUsername { get; set; }
    public string? AdministratorId { get; set; }
    public DateTime DateCreated { get; set; }
    public string? Error { get; set; } = string.Empty;

    //--------------------------// 

    #region ModelBindingCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public TrustedDeviceDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    public TrustedDeviceDto(TrustedDevice mdl)
    {
        UserId = mdl.UserId;
        DeviceFingerprint = mdl.DeviceFingerprint;
        Name = mdl.Name;
        UserAgent = mdl.UserAgent;
        LastUsedDate = mdl.LastUsedDate;
        Id = mdl.Id;
        AdministratorUsername = mdl.AdministratorUsername;
        AdministratorId = mdl.AdministratorId;
        DateCreated = mdl.DateCreated;
    }

}//Cls

