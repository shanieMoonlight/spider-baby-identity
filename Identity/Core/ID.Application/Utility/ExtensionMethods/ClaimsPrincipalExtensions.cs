using CollectionHelpers;
using ID.Application.JWT.Subscriptions;
using ID.Domain.Claims.Utils;
using ID.Domain.Entities.Teams;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ID.Application.Utility.ExtensionMethods;

public static class ClaimsPrincipalExtensions
{

    /// <summary>
    /// Checks the Email claim on this <paramref name="user"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static string? GetEmail(this ClaimsPrincipal? user) =>
        user.GetClaimValue(JwtRegisteredClaimNames.Email)
        ?? user.GetClaimValue(MyIdClaimTypes.EMAIL)
        ?? user.GetClaimValue(ClaimTypes.Email);

    //-----------------------------------//

    /// <summary>
    /// Looks for the DeviceID claim on this <paramref name="user"/> found in the subscriptions claim
    /// with name <paramref name="subscritionPlanName"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static string? GetDeviceId(this ClaimsPrincipal? user, string? subscritionPlanName)
    {
        try
        {
            if (user is null)
                return null;

            if (string.IsNullOrWhiteSpace(subscritionPlanName))
                return null;

            var subsStr = user.GetClaimValues(MyIdClaimTypes.TEAM_SUBSCRIPTIONS);
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
                .FirstOrDefault(s => s.Name == subscritionPlanName)?
                .DeviceId;

        }
        catch (Exception)
        {
            //in case it's a serialization error
            return null;
        }
    }

    //-----------------------------------//

    /// <summary>
    /// Checks the team id claim on this <paramref name="user"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static Guid? GetTeamId(this ClaimsPrincipal? user)
    {
        var teamIdStr = user.GetClaimValue(MyIdClaimTypes.TEAM_ID);
        return Guid.TryParse(teamIdStr, out var teamId) ? teamId : null;
    }

    //-----------------------------------//

    /// <summary>
    /// Checks the team id claim on this <paramref name="user"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static TeamType GetTeamType(this ClaimsPrincipal? user)
    {
        var teamTypeStr = user.GetClaimValue(MyIdClaimTypes.TEAM_TYPE);
        return Enum.TryParse(typeof(TeamType), teamTypeStr, out var teamPosition) ? (TeamType)teamPosition : TeamType.customer;
    }

    //-----------------------------------//

    /// <summary>
    /// Checks the User id claim on this <paramref name="user"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var userIdStr = user.GetClaimValue(JwtRegisteredClaimNames.Sub) 
            ?? user.GetClaimValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdStr, out var userId) ? userId : null;
    }

    //-----------------------------------//

    /// <summary>
    /// Gets the Username claim on this <paramref name="user"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static string? GetUsername(this ClaimsPrincipal user) =>
        user.GetClaimValue(MyIdClaimTypes.NAME)
        ?? user.GetClaimValue(JwtRegisteredClaimNames.Name)
        ?? user.GetClaimValue(ClaimTypes.Name)
        ?? user.GetClaimValue(ClaimTypes.NameIdentifier);

    //-----------------------------------//

    /// <summary>
    /// Can <paramref name="principal"/> add/remove/change role with value <paramref name="position"/> from other users.
    /// <para><paramref name="principal"/> must have a role with value, <paramref name="position"/> or higher.</para>
    /// </summary>
    /// <param name="principal">Current user</param>
    /// <param name="claim">The claim were looking for</param>
    /// <returns></returns>
    public static bool HasClaim(this ClaimsPrincipal principal, Claim claim) =>
        principal.HasClaim(c => c.Type == claim.Type && c.Value == claim.Value);

    //-----------------------------------//

    /// <summary>
    /// Checks if <paramref name="user"/> has logged in
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static bool IsAuthenticated(this ClaimsPrincipal user) =>
        user.Identity?.IsAuthenticated ?? false;

    //-----------------------------------//

    /// <summary>
    /// Checks if <paramref name="user"/> is in team with Id <paramref name="teamId"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static bool IsInCustomerTeam(this ClaimsPrincipal? user) =>
        user?.GetClaimValue(MyIdClaimTypes.TEAM_TYPE) == MyTeamClaimValues.CUSTOMER_TEAM_NAME;

    //-----------------------------------//

    /// <summary>
    /// Checks if <paramref name="user"/> is in Mntc team
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static bool IsInMntcTeam(this ClaimsPrincipal? user) =>
        user?.GetClaimValue(MyIdClaimTypes.TEAM_TYPE) == MyTeamClaimValues.MAINTENANCE_TEAM_NAME;

    //-----------------------------------//

    /// <summary>
    /// Checks if <paramref name="user"/> is in Mntc team or Higher
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static bool IsInMntcTeamMinimum(this ClaimsPrincipal? user) =>
        (user?.IsInMntcTeam() ?? false) || (user?.IsInSuperTeam() ?? false);

    //- - - - - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks if <paramref name="user"/> is in Customer team or Higher
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static bool IsInCustomerTeamMinimum(this ClaimsPrincipal? user) =>
        (user?.IsInCustomerTeam() ?? false) || (user?.IsInMntcTeamMinimum() ?? false);

    //-----------------------------------//

    /// <summary>
    /// Checks if <paramref name="user"/> is in team with Id <paramref name="teamId"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static bool IsInMyIdRole(this ClaimsPrincipal? principal, string role) =>
        principal?.HasClaim(c =>
        (c.Type == MyIdClaimTypes.ROLE || c.Type == ClaimTypes.Role || c.Type == "role")
        && c.Value == role)
        ?? false;

    //-----------------------------------//

    /// <summary>
    /// Checks if <paramref name="user"/> is in Super team
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static bool IsInSuperTeam(this ClaimsPrincipal? user) =>
        user?.GetClaimValue(MyIdClaimTypes.TEAM_TYPE) == MyTeamClaimValues.SUPER_TEAM_NAME;

    //-----------------------------------//

    /// <summary>
    /// Checks if <paramref name="user"/> is in team with Id <paramref name="teamId"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static bool IsInTeam(this ClaimsPrincipal? user, Guid teamId) =>
        user.GetTeamId() == teamId;

    //-----------------------------------//

    /// <summary>
    /// Checks if <paramref name="user"/> is in team with Id <paramref name="teamId"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static bool IsTeamLeader(this ClaimsPrincipal? user) =>
        IsInMyIdRole(user, MyTeamClaimValues.LEADER);

    //-----------------------------------//

    /// <summary>
    /// Checks the team id claim on this <paramref name="user"/>
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    public static int TeamPosition(this ClaimsPrincipal? user)
    {
        var teamPositionStr = user.GetClaimValue(MyIdClaimTypes.TEAM_POSITION);
        return int.TryParse(teamPositionStr, out var teamPosition) ? teamPosition : -1;
    }

    //-----------------------------------//

    public static string? GetClaimValue(this ClaimsPrincipal? principal, string claimType) =>
        principal?.FindFirst(claimType)?.Value;

    //------------------------------------//

    public static List<string>? GetClaimValues(this ClaimsPrincipal? principal, string claimType) =>
        principal?.FindAll(claimType)?.Select(c => c.Value).ToList();

    //------------------------------------//

}//Cls
