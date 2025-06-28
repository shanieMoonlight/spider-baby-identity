using ID.Application.Utility.ExtensionMethods;
using ID.TeamRoles.UserToAdmin.Claims;
using Microsoft.AspNetCore.Http;
namespace ID.TeamRoles.UserToAdmin.Authenticators.Utils;

public static class HttpContextTeamRoleExtensions
{


    /// <summary>
    /// Is a User or higher in any team
    /// </summary>
    public static bool IsUserMin(this HttpContext ctx) =>
        ctx.HasClaim(IdTeamRoleClaims.USER)
        ||
        ctx.IsMgrMin();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Is a Manager or higher in any team
    /// </summary>
    public static bool IsMgrMin(this HttpContext ctx) =>
        ctx.HasClaim(IdTeamRoleClaims.MANAGER)
        ||
        ctx.IsAdminMin();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Is a User or higher in any team
    /// </summary>
    public static bool IsAdminMin(this HttpContext ctx) =>
        ctx.HasClaim(IdTeamRoleClaims.ADMIN);

    //-----------------------------------//

    /// <summary>
    /// Is in a  team of higher trank thean the Customer team
    /// </summary>
    public static bool IsHigherThanCustomer(this HttpContext ctx) =>
      ctx.IsInSuperTeam() || ctx.IsInMntcTeam();

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Is in a  team of higher trank thean the Customer team
    /// </summary>
    public static bool IsHigherThanMntc(this HttpContext ctx) =>
      ctx.IsInSuperTeam();



}
