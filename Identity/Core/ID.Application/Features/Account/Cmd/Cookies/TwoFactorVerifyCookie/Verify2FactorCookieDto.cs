using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.Account.Cmd.Cookies.TwoFactorVerifyCookie;

public class Verify2FactorCookieDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    public string? DeviceId { get; set; }

    public Guid UserId { get; set; }

    public bool RememberMe { get; set; }

}//Cls