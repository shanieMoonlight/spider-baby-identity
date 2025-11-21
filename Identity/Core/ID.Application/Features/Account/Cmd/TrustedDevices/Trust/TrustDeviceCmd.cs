using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.TrustedDevices.Trust;

public sealed record TrustDeviceCreateDto(
    string Fingerprint,
    string Name,
    string? UserAgent,
    int? TrustDays);


public record TrustDeviceCmd(TrustDeviceCreateDto Dto) : AIdUserAwareCommand<AppUser, TrustedDeviceDto>;
