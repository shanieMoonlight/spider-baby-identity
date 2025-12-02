using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.TrustedDevices.ValueObjects;

//=============================================================================//

public sealed class IpAddress : StringValueObject
{
    public static readonly int MaxLength = 75;

    private IpAddress(string value) : base(value) { }

    public static IpAddress Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(IpAddress));
        Ensure.MaxLength(value, MaxLength, nameof(IpAddress));

        return new IpAddress(value);
    }

}


//=============================================================================//

public sealed class IpAddressNullable : NullableStringValueObject
{
    public static readonly int MaxLength = 75;

    private IpAddressNullable(string? value) : base(value) { }

    public static IpAddressNullable Create(string? value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(IpAddressNullable));
        Ensure.MaxLength(value, MaxLength, nameof(IpAddressNullable));

        return new IpAddressNullable(value);
    }

}


//=============================================================================//