using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.TrustedDevices.ValueObjects;

//=============================================================================//

public sealed class UserAgent : NullableStringValueObject
{
    public static readonly int MaxLength = 500;

    private UserAgent(string? value) : base(value) { }

    public static UserAgent Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(UserAgent));
        Ensure.MaxLength(value, MaxLength, nameof(UserAgent));
        return new UserAgent(value);
    }

    public static UserAgent CreateNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new UserAgent(null);

        Ensure.MaxLength(value, MaxLength, nameof(UserAgent));
        return new UserAgent(value);
    }

}

//=============================================================================//
