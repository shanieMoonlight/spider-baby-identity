using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.TrustedDevices.ValueObjects;

//=============================================================================//

public sealed class UserAgent : StringValueObject
{
    public static readonly int MaxLength = 100;

    private UserAgent(string value) : base(value) { }

    public static UserAgent Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(UserAgent));
        Ensure.MaxLength(value, MaxLength, nameof(UserAgent));

        return new UserAgent(value);
    }

}


//=============================================================================//

public sealed class UserAgentNullable : NullableStringValueObject
{
    public static readonly int MaxLength = 100;

    private UserAgentNullable(string? value) : base(value) { }

    public static UserAgentNullable Create(string? value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(UserAgentNullable));
        Ensure.MaxLength(value, MaxLength, nameof(UserAgentNullable));

        return new UserAgentNullable(value);
    }

}


//=============================================================================//
