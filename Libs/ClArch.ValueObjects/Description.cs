using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;

namespace ClArch.ValueObjects;

//==========================================//

public class Description : StringInvariantValueObject
{

    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthDescription;

    private Description(string value) : base(value) { }

    public static Description Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(Description));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(Description));

        return new(value);
    }

}//Cls

//==========================================//

public class DescriptionNullable : NullableStringInvariantValueObject
{

    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthDescription;

    private DescriptionNullable(string? value) : base(value) { }

    public static DescriptionNullable Create(string? value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(Description));
        return new(value);
    }

}//Cls

//==========================================//
