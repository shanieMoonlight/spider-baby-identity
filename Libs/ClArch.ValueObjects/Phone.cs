using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;
using StringHelpers;
using ValidationHelpers;

namespace ClArch.ValueObjects;

//====================================================//

public class Phone : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthPhone;

    private Phone(string value) : base(value) { }

    public static Phone Create(string value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(Phone));

        if (!PhoneValidator.IsValid(value))
            throw new InvalidPropertyException(nameof(Phone), $"The Phone number: {value} is not valid.");

        return new(value);
    }

}//Cls

//====================================================//

public class PhoneNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthPhone;

    private PhoneNullable(string? value) : base(value) { }

    public static PhoneNullable Create(string? value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(Phone));

        if (!PhoneValidator.IsValid(value, true))
            throw new InvalidPropertyException(nameof(Phone), $"The Phone number: {value} is not valid.");

        return new(value.IsNullOrWhiteSpace() ? null : value); //Don't store white spaces
    }

}//Cls

//====================================================//

public class PhoneNullableSafe : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthPhone;

    private PhoneNullableSafe(string? value) : base(value) { }

    public static PhoneNullableSafe Create(string? value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(Phone));

        if (!PhoneValidator.IsValid(value, true))
            value = null;

        return new(value.IsNullOrWhiteSpace() ? null : value); //Don't store white spaces
    }

}//Cls

//====================================================//