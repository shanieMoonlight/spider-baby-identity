using ID.Domain.Claims.AuthMethods;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using System.Security.Claims;

namespace ID.Infrastructure.Claims.Services.Abs;

/// <summary>
/// Interface for building claims for a user.
/// </summary>
public interface IClaimsBuilderService
{

    /// <summary>
    /// Builds a list of claims for a user.
    /// </summary>
    /// <param name="user">The user for whom the claims are being built.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="currentDeviceId">The ID of the current device.</param>
    Task<List<Claim>> BuildClaimsAsync(
        AppUser user, 
        Team team,
        IEnumerable<AuthMethodRef> authMethods,
        string? currentDeviceId);
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of claims.</returns>
    //Task<List<Claim>> BuildClaimsAsync(AppUser user, Team team, bool twoFactorVerified, string? currentDeviceId);
}
