using ID.Application.Features.IdRefreshTokens;
using ID.Domain.Entities.Refreshing;

namespace ID.Application.Features.IdRefreshTokens;
public class IdRefreshTokenDto
{
    public Guid Id { get; set; }

    public string Payload { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
    public DateTime CreatedUtc { get; set; }
    public Guid? TrustedDeviceId { get; set; }
    public TrustedDeviceDto? TrustedDevice { get; set; }
    public bool IsExpired { get; set; }
    public DateTime DateCreated { get; set; }
    public string? Error { get; set; } = string.Empty;

    //--------------------------// 

    #region ModelBindingCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IdRefreshTokenDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    public IdRefreshTokenDto(IdRefreshToken mdl)
    {
        Payload = mdl.PayloadHash;
        ExpiresOnUtc = mdl.ExpiresOnUtc;
        CreatedUtc = mdl.CreatedUtc;
        TrustedDeviceId = mdl.TrustedDeviceId;
        TrustedDevice = mdl.TrustedDevice?.ToDto();
        IsExpired = mdl.IsExpired;
        Id = mdl.Id;
        DateCreated = mdl.DateCreated;

        //  Error = mdl.Error;
    }


}//Cls

