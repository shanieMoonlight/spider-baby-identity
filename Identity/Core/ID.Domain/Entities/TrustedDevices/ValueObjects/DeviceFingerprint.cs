using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.TrustedDevices.ValueObjects;

//=============================================================================//

public sealed class DeviceFingerprint : StringValueObject
{
    public static readonly int MaxLength = 512;

    private DeviceFingerprint(string value) : base(value) { }

    public static DeviceFingerprint Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(DeviceFingerprint));
        Ensure.MaxLength(value, MaxLength, nameof(DeviceFingerprint));

        return new DeviceFingerprint(value);
    }

}

//=============================================================================//
