using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using MyResults;

namespace ID.Application.AppAbs.ApplicationServices.TwoFactor;



/// <summary>
/// Interface for sending OTP for 2-factor authentication
/// </summary>
public interface ITwoFactorMsgService
{
    /// <summary>
    /// Sends an OTP for 2-factor authentication to the specified user in the specified team.
    /// </summary>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="user">The user to whom the OTP will be sent.</param>
    /// <param name="provider">The provider to be used for sending the OTP. If null, the default provider will be used.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the OTP sending operation.</returns>
    Task<GenResult<MfaResultData>> SendOTPFor2FactorAuth(Team team, AppUser user, TwoFactorProvider? provider = null);
}
