using ID.Domain.Claims.Utils;
using ID.Domain.Entities.Teams;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ID.Application.JWT.Subscriptions;

/// <summary>
/// Provides extension methods for <see cref="TeamSubscription"/> to convert to claims.
/// </summary>
public static class SubscriptionClaimDataExtensions
{
    /// <summary>
    /// Converts a <see cref="TeamSubscription"/> to a <see cref="Claim"/>.
    /// </summary>
    /// <param name="sub">The team subscription.</param>
    /// <returns>The claim representing the team subscription.</returns>
    public static Claim ToClaim(this TeamSubscription sub, string? currentDeviceId) =>
        SubscriptionClaimData.Create(sub, currentDeviceId).ToClaim();

    //----------------------//

    public static Claim ToClaim(this SubscriptionClaimData data) =>
        new(MyIdClaimTypes.TEAM_SUBSCRIPTIONS, data.Serialize(), JsonClaimValueTypes.Json);

    //----------------------//

    /// <summary>
    /// Converts a collection of <see cref="TeamSubscription"/> to a list of <see cref="Claim"/>.
    /// </summary>
    /// <param name="subs">The collection of team subscriptions.</param>
    /// <returns>The list of claims representing the team subscriptions.</returns>
    public static List<Claim> ToClaims(this IEnumerable<TeamSubscription> subs, string? currentDeviceId)
    {
        List<Claim> claims = [];
        foreach (var subscription in subs ?? [])
        {
            claims.Add(subscription.ToClaim(currentDeviceId));
        }
        return claims;
    }

    //----------------------//

    /// <summary>
    /// Serializes the subscription claim data to a JSON string.
    /// </summary>
    /// <returns>The JSON string representation of the subscription claim data.</returns>
    internal static string Serialize(this SubscriptionClaimData data) => JsonConvert.SerializeObject(data, new JsonSerializerSettings
    {
        ContractResolver = new SubscriptionClaimDataContractResolver()
    });


}//Cls

