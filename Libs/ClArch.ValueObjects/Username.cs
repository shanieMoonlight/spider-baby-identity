using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;
using StringHelpers;

namespace ClArch.ValueObjects;

//==========================================//

public class Username : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthDescription;

    private Username(string value) : base(value) { }

    public static Username Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(Username));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(Username));

        return new(value); 
    }

}//Cls

//==========================================//

public class UsernameNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthDescription;

    private UsernameNullable(string? value) : base(value) { }

    public static UsernameNullable Create(string? value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(Username));

        return new(value.IsNullOrWhiteSpace() ? null : value); //Don't store white spaces
    }

}//Cls

//==========================================//


