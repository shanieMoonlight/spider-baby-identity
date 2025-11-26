using ID.Domain.Claims.AuthMethods;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Infrastructure.Auth.JWT.LocalServices.Abs;
public interface IJwtBuilder
{
    Task<string> CreateJwtAsync(
        AppUser user, 
        Team team,
        IEnumerable<AuthMethodRef> authMethods,
        string? currentDeviceFingerprint = null);
}