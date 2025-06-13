using MyResults;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppAbs.ApplicationServices.TwoFactor;

/// <summary>
/// Implemented in Application Layer
/// </summary>
public interface ITwoFactorCompleteRegistrationHandler
{
    /// <summary>
    /// USes <paramref name="twoFactorCode"/> to verify the 2-factor provider that this <paramref name="user"/> has set up
    /// </summary>
    Task<BasicResult> EnableAsync(AppUser user, string twoFactorCode);

}