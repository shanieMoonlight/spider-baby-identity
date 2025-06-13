using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Models;

namespace ID.Application.Features.Account.Cmd.LoginRefresh;
public record LoginRefreshCmd(string RefreshToken, string? DeviceId) : AIdCommand<JwtPackage>;



