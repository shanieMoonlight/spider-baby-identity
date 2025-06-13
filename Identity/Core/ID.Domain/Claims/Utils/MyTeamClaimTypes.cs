using ID.GlobalSettings.Setup.Settings;


namespace ID.Domain.Claims.Utils;

public class MyTeamClaimTypes
{
    public static readonly string CLAIM_TYPE_PREFIX = IdGlobalSettings.ClaimTypePrefix;


    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
    //=-=-=-=-=-=-=-=-=-=-=-=-  POSITION IN TEAM  =-=-=-=-=-=-=-=-=-=-=-=//
    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//


    ///// <summary>
    ///// Team leader, Highest access level. Creator of team.
    ///// </summary>
    //public static readonly string LEADER = $"{CLAIM_TYPE_PREFIX}.team_leader";


    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
    //=-=-=-=-=-=-=-=-=-=-=-=-=      TEAMS       =-=-=-=-=-=-=-=-=-=-=-=-//
    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//


    /// <summary>
    /// Part of the Super Team 
    /// </summary>
    public static readonly string SUPER = $"{CLAIM_TYPE_PREFIX}.roles.super";

    /// <summary>
    /// Part of the App Maintenance Team 
    /// </summary>
    public static readonly string MAINTENANCE = $"{CLAIM_TYPE_PREFIX}.roles.maintenance";

    /// <summary>
    /// Customer of company. Can register themselves.  
    /// </summary>
    public static readonly string CUSTOMER = $"{CLAIM_TYPE_PREFIX}.roles.customer";


    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

    /// <summary>
    ///  Used to locate Roles in JWT
    /// </summary>
    public static readonly string KEY = $"{CLAIM_TYPE_PREFIX}.Role";

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

}//Cls