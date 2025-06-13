using ClArch.ValueObjects.Common;

namespace ID.Domain.Entities.SubscriptionPlans.ValueObjects;
public class TrialMonths : ValueObject<int>
{
    public const int MinTrialMonths = 0;

    private TrialMonths(int value) : base(value) { }

    public static TrialMonths Create(int? value) =>
        new(Math.Max(value ?? 0, MinTrialMonths));

}//Cls