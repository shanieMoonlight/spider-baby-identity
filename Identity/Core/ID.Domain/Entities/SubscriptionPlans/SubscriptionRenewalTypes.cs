namespace ID.Domain.Entities.SubscriptionPlans;

public enum SubscriptionRenewalTypes
{

    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Quarterly = 4,
    Biannual = 5,
    Annual = 6,
    Lifetime = 7

}//Enm


//#################################################//


internal static class SubscriptionRenewalTypesExtensions
{
    /// <summary>
    /// Extends the given date by the specified renewal type.
    /// </summary>
    /// <param name="from">The starting date.</param>
    /// <param name="renewalTypes">The type of renewal.</param>
    /// <returns>The extended date or null for lifetime renewals.</returns>
    internal static DateTime? Extend(this DateTime from, SubscriptionRenewalTypes renewalTypes) =>
        renewalTypes switch
        {
            SubscriptionRenewalTypes.Daily => from.AddDays(1),
            SubscriptionRenewalTypes.Weekly => from.AddDays(7),
            SubscriptionRenewalTypes.Monthly => from.AddMonths(1),
            SubscriptionRenewalTypes.Quarterly => from.AddMonths(3),
            SubscriptionRenewalTypes.Biannual => from.AddMonths(6),
            SubscriptionRenewalTypes.Annual => from.AddYears(1),
            SubscriptionRenewalTypes.Lifetime => null,
            _ => from.AddDays(1),
        };

    //----------------------------//

    /// <summary>
    /// Extends the given nullable date by the specified renewal type.
    /// </summary>
    /// <param name="from">The starting date.</param>
    /// <param name="renewalTypes">The type of renewal.</param>
    /// <returns>The extended date or null for lifetime renewals.</returns>
    internal static DateTime? Extend(this DateTime? from, SubscriptionRenewalTypes renewalTypes) =>
        (from ?? DateTime.Now).Extend(renewalTypes);

    //----------------------------//

    /// <summary>
    /// Determines whether the subscription has expired based on the last payment date and renewal type.
    /// </summary>
    /// <param name="lastPaymentDate">The last payment date.</param>
    /// <param name="renewalTypes">The type of renewal.</param>
    /// <returns>True if the subscription has expired; otherwise, false.</returns>

    internal static bool HasExpired(this DateTime? lastPaymentDate, SubscriptionRenewalTypes renewalTypes)
    {
        if (lastPaymentDate is null)
            return true;

        var nextPaymentDate = lastPaymentDate.Value.Extend(renewalTypes);
        return nextPaymentDate < DateTime.Now;
    }
}