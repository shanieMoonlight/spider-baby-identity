using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Models;
using MyResults;

namespace ID.Application.AppImps.TwoFactor;
internal class TwoFactorCompleteRegistrationHandler(IAuthenticatorAppService authy)
    : ITwoFactorCompleteRegistrationHandler
{
    public async Task<BasicResult> EnableAsync(AppUser user, string twoFactorCode)
    {
        return user.TwoFactorProvider switch
        {
            TwoFactorProvider.AuthenticatorApp => await authy.EnableAsync(user, twoFactorCode),
            _ => BasicResult.Success(), //Otherwise do nothing
        };

    }
}//Cls
