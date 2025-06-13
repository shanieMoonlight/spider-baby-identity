using ID.Domain.Entities.AppUsers;
using System.Security.Claims;

namespace ID.Infrastructure.Auth.JWT.LocalServices.Abs;
public interface IJwtClaimsService
{
    List<Claim> AddRegisteredClaims<TUser>(IEnumerable<Claim> initialClaims, TUser user) where TUser : AppUser;
}