using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;

public class Verify2FactorDto
{
    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    public string? DeviceId { get; set; }
    
    // Optional: device fingerprint to create a trusted-device entry when requested
    public string? DeviceFingerprint { get; set; }

    // When true, create a trusted device record for this device after successful verification
    public bool TrustDevice { get; set; } = false;

    //public Guid UserId { get; set; }

}//Cls