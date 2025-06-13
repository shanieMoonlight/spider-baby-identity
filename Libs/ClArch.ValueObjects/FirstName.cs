using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;
using StringHelpers;

namespace ClArch.ValueObjects;

//====================================================//

public class FirstName : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthFirstName;

    private FirstName(string value) : base(value) { }

    public static FirstName Create(string value)
    {

        Ensure.NotNullOrWhiteSpace(value, nameof(FirstName));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(FirstName));

        return new(value);
    }

}//Cls

//====================================================//


public class FirstNameNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthFirstName;

    private FirstNameNullable(string? value) : base(value) { }

    public static FirstNameNullable Create(string? value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(FirstName));
        
        return new(value.IsNullOrWhiteSpace() ? null : value);
    }

}//Cls

//====================================================//



