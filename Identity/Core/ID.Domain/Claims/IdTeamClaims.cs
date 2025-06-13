using ID.Domain.Claims.Utils;
using ID.GlobalSettings.Setup.Defaults;
using System.Security.Claims;

namespace ID.Domain.Claims;


//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

public class IdTeamClaim : Claim
{
    private IdTeamClaim(string type, string value, string? valueType, string? issuer, string? originalIssuer)
        : base(type, value, valueType, issuer, originalIssuer) { }

    //----------------------------------------------------//

    public static IdTeamClaim GenerateRoleClaim(string roleValue) =>
        GenerateClaim(MyIdClaimTypes.ROLE, roleValue);

    //----------------------------------------------------//

    public static IdTeamClaim GenerateRoleClaim(int roleValue) =>
        GenerateClaim(MyIdClaimTypes.ROLE, $"{roleValue}");

    //----------------------------------------------------//

    public static IdTeamClaim GenerateTeamTypeClaim(string teamType) =>
        GenerateClaim(MyIdClaimTypes.TEAM_TYPE, teamType);

    //----------------------------------------------------//

    public static IdTeamClaim GenerateTeamPositionClaim(int position) =>
        GenerateClaim(MyIdClaimTypes.TEAM_POSITION, position.ToString());

    //----------------------------------------------------//

    public static IdTeamClaim GenerateTeamSubscriptionClaim(int position) =>
        GenerateClaim(MyIdClaimTypes.TEAM_POSITION, position.ToString());

    //----------------------------------------------------//

    public static IdTeamClaim GenerateClaim(string claimType, string claimValue, string valueType = ClaimValueTypes.String) =>
        new(claimType, claimValue, valueType, IdGlobalDefaultValues.TOKEN_ISSUER, IdGlobalDefaultValues.TOKEN_ISSUER);


}//Cls

//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

public class IdTeamClaims
{
    /// <summary>
    /// Create team position claim
    /// </summary>
    public static IdTeamClaim TEAM_POSITION(int position) => IdTeamClaim.GenerateTeamPositionClaim(position);

    /// <summary>
    /// Create team position claim
    /// </summary>
    public static IdTeamClaim TEAM_ID<T>(T id) where T : notnull => IdTeamClaim.GenerateClaim(MyIdClaimTypes.TEAM_ID, $"{id}");

    /// <summary>
    /// <inheritdoc cref="MyTeamClaimTypes.LEADER/>
    /// </summary>
    public readonly static IdTeamClaim LEADER = IdTeamClaim.GenerateRoleClaim(MyTeamClaimValues.LEADER);

    /// <summary>
    /// <inheritdoc cref="MyTeamClaimTypes.SUPER"/>
    /// </summary>
    public readonly static IdTeamClaim SUPER_TEAM = IdTeamClaim.GenerateTeamTypeClaim(MyTeamClaimValues.SUPER_TEAM_NAME);

    /// <summary>
    /// <inheritdoc cref="MyTeamClaimTypes.MAINTENANCE"/> 
    /// </summary>
    public readonly static IdTeamClaim MAINTENANCE_TEAM = IdTeamClaim.GenerateTeamTypeClaim(MyTeamClaimValues.MAINTENANCE_TEAM_NAME);

    /// <summary>
    /// <inheritdoc cref="MyTeamClaimTypes.CUSTOMER"/> 
    /// </summary>
    public readonly static IdTeamClaim CUSTOMER_TEAM = IdTeamClaim.GenerateTeamTypeClaim(MyTeamClaimValues.CUSTOMER_TEAM_NAME);

}//Cls


//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//