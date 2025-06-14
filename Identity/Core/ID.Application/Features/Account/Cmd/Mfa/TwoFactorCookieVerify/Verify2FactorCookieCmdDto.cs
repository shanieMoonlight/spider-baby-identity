using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorCookieVerify;

public class Verify2FactorCookieCmdDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    public string? DeviceId { get; set; }
    public bool RememberMe { get; set; }

}//Cls