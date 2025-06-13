using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;

namespace ClArch.ValueObjects;

//========================//

public class AddressLine : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthAddressLine;

    private AddressLine(string value) : base(value) { }

    public static AddressLine Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(AddressLine));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(AddressLine));

        return new(value);
    }

}//Cls

//========================//

public class AddressLineNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthAddressLine;

    private AddressLineNullable(string? value) : base(value) { }

    public static AddressLineNullable Create(string? value)
    {

        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(AddressLine));

        return new(value);
    }

}//Cls


//========================//
