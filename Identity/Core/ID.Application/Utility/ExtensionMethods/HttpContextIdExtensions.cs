using CollectionHelpers;
using ID.Application.JWT.Subscriptions;
using ID.Domain.Claims.Utils;
using ID.Domain.Entities.Teams;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ID.Application.Utility.ExtensionMethods;

public static class HttpContextIdExtensions
{

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetDeviceId(ClaimsPrincipal, string)"/>
    /// </summary>
    public static string? GetDeviceId(this HttpContext ctx, string? subscritionPlanName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(subscritionPlanName))
                return null;

            var subsStr = ctx.User.GetClaimValues(MyIdClaimTypes.TEAM_SUBSCRIPTIONS);
            if (!subsStr.AnyValues())
                return null;

            List<SubscriptionClaimData> subscriptionClaimDatas = [];

            foreach (var sub in subsStr!)
            {
                var deserializedData = JsonConvert.DeserializeObject<SubscriptionClaimData>(sub, new JsonSerializerSettings
                {
                    ContractResolver = new SubscriptionClaimDataContractResolver()
                });

                if (deserializedData is not null)
                    subscriptionClaimDatas.Add(deserializedData);
            }

            return subscriptionClaimDatas?
                .FirstOrDefault(s => s.Name.Equals(subscritionPlanName, StringComparison.CurrentCultureIgnoreCase))?
                .DeviceId;

        }
        catch (Exception)
        {
            return null;
        }
    }

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetEmail(ClaimsPrincipal)"/>
    /// </summary>
    public static string? GetEmail(this HttpContext ctx) =>
        ctx.User.GetClaimValue(ClaimTypes.Email) ?? ctx.User.GetClaimValue(ClaimTypes.NameIdentifier);

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetTeamId(ClaimsPrincipal)"/>
    /// </summary>
    public static Guid? GetTeamId(this HttpContext? ctx)
    {
        var teamIdStr = ctx?.User.GetClaimValue(MyIdClaimTypes.TEAM_ID);
        return Guid.TryParse(teamIdStr, out var teamId) ? teamId : null;
    }

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetTeamType(ClaimsPrincipal)"/>
    /// </summary>
    public static TeamType GetTeamType(this HttpContext? ctx)
    {
        var teamTypeStr = ctx?.User.GetClaimValue(MyIdClaimTypes.TEAM_TYPE);
        return Enum.TryParse(typeof(TeamType), teamTypeStr, out var teamPosition) ? (TeamType)teamPosition : TeamType.customer;
    }

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetUserId(ClaimsPrincipal)"/>
    /// </summary>
    public static Guid? GetUserId(this HttpContext ctx)
    {
        var userIdStr = ctx.User.GetClaimValue(JwtRegisteredClaimNames.Sub) ?? ctx.User.GetClaimValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdStr, out var userId) ? userId : null;
    }

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.GetUsername(ClaimsPrincipal)"/>
    /// </summary>
    public static string? GetUsername(this HttpContext ctx) =>
        ctx.User.GetClaimValue(ClaimTypes.Name) ?? ctx.User.GetClaimValue(ClaimTypes.NameIdentifier);

    //----------------------//

    /// <summary>
    /// <inheritdoc cref="ClaimsPrincipalExtensions.HasClaim(ClaimsPrincipal, Claim)"/>
    /// </summary>
    public static bool HasClaim(this HttpContext ctx, Claim claim) =>
        ctx.User.HasClaim(c => c.Type == claim.Type && c.Value == claim.Value);

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
        ctx?.User.GetClaimValue(MyIdClaimTypes.TEAM_TYPE) == MyTeamClaimValues.CUSTOMER_TEAM_NAME;

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
    /// <inheritdoc cref="ClaimsPrincipalExtensions.IsInMyRole(ClaimsPrincipal, string)"/>
    /// </summary>
    public static bool IsInMyRole(this HttpContext ctx, string role) =>
        ctx.User.HasClaim(c => c.Type == MyIdClaimTypes.ROLE && c.Value == role);

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
    public static int TeamPosition(this HttpContext ctx)=> 
        ctx?.User?.TeamPosition() ?? -1;
}
