using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.Account.Cmd.LoginRefresh;

public class LoginRefreshDto
{
    [Required]
    public string ResetToken { get; set; } = string.Empty;

    public string? DeviceId { get; set; }

}//Cls