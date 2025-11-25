using System.ComponentModel.DataAnnotations;
using ID.Domain.Models;

namespace ID.Application.Features.Account.Cmd.LoginRefresh;

public class LoginRefreshDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;

    public string? DeviceFingerprint { get; set; }

}

public record LoginRefreshCmd(LoginRefreshDto Dto) : AIdCommand<JwtPackage>;



