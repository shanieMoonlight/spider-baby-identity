using MyResults;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppAbs.MFA.AuthenticatorApps;
public interface IAuthenticatorAppService
{
    Task<BasicResult> EnableAsync(AppUser user, string twoFactorCode);
    Task<AuthAppSetupDto> Setup(AppUser user);
    Task<bool> ValidateAsync(AppUser user, string twoFactorCode);
}