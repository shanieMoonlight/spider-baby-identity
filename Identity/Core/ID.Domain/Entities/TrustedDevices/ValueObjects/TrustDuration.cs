namespace ID.Domain.Entities.TrustedDevices.ValueObjects;

//=============================================================================//

using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;


public class TrustDuration : ValueObject<TimeSpan>
{
    private TrustDuration(TimeSpan value) : base(value) { }

    public static TrustDuration Create(TimeSpan value)
    {
        Ensure.MinValue(value, TimeSpan.Zero, nameof(TrustDuration));

        return new(value);
    }

}//Cls

//=============================================================================//

public class TrustDurationNullable : ValueObject<TimeSpan?>
{
    private TrustDurationNullable(TimeSpan? value) : base(value) { }

    public static TrustDurationNullable Create(TimeSpan? value)
    {
        Ensure.MinValueNullable(value, TimeSpan.Zero, nameof(TrustDuration));

        return new(value);
    }

}//Cls


//=============================================================================//