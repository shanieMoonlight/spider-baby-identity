using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;

public class Verify2FactorDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    public string? DeviceId { get; set; }

}//Cls