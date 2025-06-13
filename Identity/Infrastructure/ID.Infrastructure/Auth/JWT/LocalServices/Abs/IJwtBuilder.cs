using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Infrastructure.Auth.JWT.LocalServices.Abs;
public interface IJwtBuilder
{
    Task<string> CreateJwtAsync(AppUser user, Team team, bool twoFactorVerified, string? currentDeviceId = null);
    Task<string> CreateJwtWithTwoFactorRequiredAsync(AppUser user, Team team, string? currentDeviceId = null);
}