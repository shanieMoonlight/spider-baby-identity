using ID.Domain.Entities.Teams;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ID.Application.Utility.ExtensionMethods;

public static class HttpContextIdExtensions
{

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetDeviceId(ClaimsPrincipal, string)"/>
    /// </summary>
    public static string? GetDeviceId(this HttpContext ctx, string? subscritionPlanName) =>
        ctx.User.GetDeviceId(subscritionPlanName);

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetEmail(ClaimsPrincipal)"/>
    /// </summary>
    public static string? GetEmail(this HttpContext ctx) =>
        ctx.User.GetEmail();

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetTeamId(ClaimsPrincipal)"/>
    /// </summary>
    public static Guid? GetTeamId(this HttpContext? ctx) =>
        ctx?.User.GetTeamId();

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetTeamType(ClaimsPrincipal)"/>
    /// </summary>
    public static TeamType GetTeamType(this HttpContext? ctx) =>
        ctx?.User.GetTeamType() ?? TeamType.customer;

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetUserId(ClaimsPrincipal)"/>
    /// </summary>
    public static Guid? GetUserId(this HttpContext ctx) =>
               ctx.User.GetUserId();

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetUsername(ClaimsPrincipal)"/>
    /// </summary>
    public static string? GetUsername(this HttpContext ctx) =>
         ctx.User.GetUsername();

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.HasClaim(ClaimsPrincipal, Claim)"/>
    /// </summary>
    public static bool HasClaim(this HttpContext ctx, Claim claim) =>
         ctx.User.HasClaim(claim);

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.IsAuthenticated()"/>
    /// </summary>
    public static bool IsAuthenticated(this HttpContext ctx) =>
        ctx.User.IsAuthenticated();

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.IsInCustomerTeam(ClaimsPrincipal)"/>
    /// </summary>
    public static bool IsInCustomerTeam(this HttpContext ctx) =>
        ctx?.User.IsInCustomerTeam() ?? false;

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.IsInMntcTeam(ClaimsPrincipal)"/>
    /// </summary>
    public static bool IsInMntcTeam(this HttpContext ctx) =>
        ctx?.User.IsInMntcTeam() ?? false;

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.IsInMntcTeam(ClaimsPrincipal)"/>
    /// </summary>
    public static bool IsInMntcTeamMinimum(this HttpContext ctx) =>
        ctx?.User.IsInMntcTeamMinimum() ?? false;

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.IsInMyIdRole(ClaimsPrincipal, string)"/>
    /// </summary>
    public static bool IsInRole(this HttpContext ctx, string role) =>
        ctx?.User.IsInRole(role)
        ?? false;

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.IsInMyIdRole(ClaimsPrincipal, string)"/>
    /// </summary>
    public static bool IsInMyIdRole(this HttpContext ctx, string role) =>
        ctx?.User.IsInRole(role)
        ?? false;

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.IsInSuperTeam(ClaimsPrincipal)"/>
    /// </summary>
    public static bool IsInSuperTeam(this HttpContext? ctx) =>
        ctx?.User.IsInSuperTeam() ?? false;

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.IsInTeam(ClaimsPrincipal, Guid)"/>
    /// </summary>
    public static bool IsInTeam(this HttpContext ctx, Guid teamId) =>
        ctx?.User?.IsInTeam(teamId) ?? false;

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.IsTeamLeader(ClaimsPrincipal)"/>
    /// </summary>
    public static bool IsTeamLeader(this HttpContext ctx) =>
        ctx?.User?.IsTeamLeader() ?? false;

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.TeamPosition(ClaimsPrincipal)"/>
    /// </summary>
    public static int TeamPosition(this HttpContext ctx) =>
        ctx?.User?.TeamPosition() ?? -1;
}
