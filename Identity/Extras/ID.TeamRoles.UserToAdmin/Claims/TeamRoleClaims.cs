using ID.Domain.Claims;

namespace ID.TeamRoles.UserToAdmin.Claims;
public class IdTeamRoleClaims
{

    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

    /// <summary>
    /// <inheritdoc cref="MyTeamClaimValues.ADMIN"/>
    /// </summary>
    public readonly static IdTeamClaim ADMIN = IdTeamClaim.GenerateRoleClaim(MyTeamRoleClaimValues.ADMIN);

    /// <summary>
    /// <inheritdoc cref="MyTeamRoleClaimValues.MANAGER"/>
    /// </summary>
    public readonly static IdTeamClaim MANAGER = IdTeamClaim.GenerateRoleClaim(MyTeamRoleClaimValues.MANAGER);

    /// <summary>
    /// <inheritdoc cref="MyTeamRoleClaimValues.USER"/>
    /// </summary>
    public readonly static IdTeamClaim USER = IdTeamClaim.GenerateRoleClaim(MyTeamRoleClaimValues.USER);


    //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

}//Cls