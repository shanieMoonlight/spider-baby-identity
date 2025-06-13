using ClArch.ValueObjects.Common;

namespace ID.Domain.Entities.SubscriptionPlans.ValueObjects;
public class DeviceLimit : ValueObject<int>
{
    public const int MinLimit = 0;

    private DeviceLimit(int value) : base(value) { }

    public static DeviceLimit Create(int? value) =>
        new(Math.Max(value ?? 0, MinLimit));

}//Cls