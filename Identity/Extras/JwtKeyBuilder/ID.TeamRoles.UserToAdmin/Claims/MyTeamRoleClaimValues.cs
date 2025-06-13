namespace ID.TeamRoles.UserToAdmin.Claims;

public class MyTeamRoleClaimValues
{

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
    //=-=-=-=-=-=-=-=-=-=-=-=-  POSITION IN TEAM  =-=-=-=-=-=-=-=-=-=-=-=//
    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

    /// <summary>
    /// Second Highest access level on a team
    /// </summary>
    public static readonly string ADMIN = $"admin";

    /// <summary>
    /// Magmt access on tea, can create/delete other users.
    /// </summary>
    public static readonly string MANAGER = $"mananger";

    /// <summary>
    /// Basic user in team. Limited access.
    /// </summary>
    public static readonly string USER = $"user";

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

}//Cls