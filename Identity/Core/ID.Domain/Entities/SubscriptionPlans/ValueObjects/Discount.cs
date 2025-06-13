using ClArch.ValueObjects.Common;

namespace ID.Domain.Entities.SubscriptionPlans.ValueObjects;
public class Discount : ValueObject<double>
{
    public const int MinLimit = 0;

    private Discount(double value) : base(value) { }

    public static Discount Create(double? value) =>
        new(Math.Max(value ?? 0, MinLimit));

}//Cls