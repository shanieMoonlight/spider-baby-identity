using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Models;
using System.Text.Json.Serialization;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorUpdateMethod;

public record UpdateTwoFactorProviderDto(
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    TwoFactorProvider Provider
);
public record UpdateTwoFactorProviderCmd(UpdateTwoFactorProviderDto Dto) : AIdUserAndTeamAwareCommand<AppUser, AppUserDto>;



