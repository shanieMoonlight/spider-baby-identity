// IdGlobalConstants.Teams.cs
namespace ID.GlobalSettings.Constants;

internal partial class IdGlobalConstants
{
    internal static partial class Teams
    {
        // Name of Super Team
        /// <summary>
        ///  "Super Team"
        /// </summary>
        internal const string SUPER_TEAM_NAME = "Super Team";
        internal const string SUPER_TEAM_DESCRIPTION = "Team of SuperUser and SuperAdmins, who have God-like access.";

        internal const string MAINTENANCE_TEAM_NAME = "Maintenance Team";
        internal const string MAINTENANCE_TEAM_DESCRIPTION = "Team of users with maintenance rights/access to the data/app. Higher than regular customers.";

        /// <summary>
        /// Represents the default maximum position. Use to get members with all positions.
        /// </summary>
        internal const int CATCH_ALL_MAX_POSITION = 10000;
    }
}
