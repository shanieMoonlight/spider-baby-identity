using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.TrustedDevices.ValueObjects;

//=============================================================================//

public sealed class DeviceName : StringValueObject
{
    public static readonly int MaxLength = 100;

    private DeviceName(string value) : base(value) { }

    public static DeviceName Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(DeviceName));
        Ensure.MaxLength(value, MaxLength, nameof(DeviceName));

        return new DeviceName(value);
    }

}


//=============================================================================//

public sealed class DeviceNameNullable : NullableStringValueObject
{
    public static readonly int MaxLength = 100;

    private DeviceNameNullable(string? value) : base(value) { }

    public static DeviceNameNullable Create(string? value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(DeviceNameNullable));
        Ensure.MaxLength(value, MaxLength, nameof(DeviceNameNullable));

        return new DeviceNameNullable(value);
    }

}


//=============================================================================//