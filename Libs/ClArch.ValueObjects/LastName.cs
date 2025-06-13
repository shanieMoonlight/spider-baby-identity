using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;
using StringHelpers;

namespace ClArch.ValueObjects;

//==========================================//

public class LastName : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthLastName;

    private LastName(string value) : base(value) { }

    public static LastName Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(LastName));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(LastName));

        return new(value);
    }

}//Cls

//==========================================//

public class LastNameNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthLastName;

    private LastNameNullable(string? value) : base(value) { }

    public static LastNameNullable Create(string? value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(LastName));

        return new(value.IsNullOrWhiteSpace() ? null : value); //Don't store white spaces
    }

}//Cls

//==========================================//


