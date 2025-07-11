using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.Account.Cmd.LoginRefresh;

public class LoginRefreshDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;

    public string? DeviceId { get; set; }

}//Cls