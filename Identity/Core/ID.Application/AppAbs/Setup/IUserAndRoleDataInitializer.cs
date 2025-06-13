using ID.GlobalSettings.Constants;

namespace ID.Application.AppAbs.Setup;

public interface IUserAndRoleDataInitializer
{
    Task<bool> IsAlreadyInitializedAsync();

    /// <summary>
    /// Seed data for users and roles.
    /// Sets up super leader and super team.
    /// </summary>
    /// <param name="superLeaderPassword">Password for Super leader. </param>
    /// <param name="superLeaderEmail">Email for super leader. Default = <see cref="IdGlobalConstants.Initialization.SUPER_LEADER_EMAIL"/> </param>
    /// <returns>superLeaderEmail</returns>
    Task<string> SeedDataAsync(string superLeaderPassword, string? superLeaderEmail = null);
}