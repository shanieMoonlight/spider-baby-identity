using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;
using ValidationHelpers;

namespace ClArch.ValueObjects;

//==========================================//

public class EmailAddress : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthEmail;

    private EmailAddress(string value) : base(value) { }

    public static EmailAddress Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(EmailAddress));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(EmailAddress));

        if (!EmailValidator.IsValid(value))
            throw new InvalidPropertyException(nameof(EmailAddressNullable), $"The Email address: {value} is not valid.");

        return new(value.ToLower());//Store as lower case for consistency
    }

}//Cls

//==========================================//

public class EmailAddressNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthEmail;

    private EmailAddressNullable(string? value) : base(value) { }

    public static EmailAddressNullable Create(string? value)
    {

        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(EmailAddress));

        if (!EmailValidator.IsValid(value))
            throw new InvalidPropertyException(nameof(EmailAddressNullable), $"The Email address: {value} is not valid.");


        return new(value?.ToLower()); //Store as lower case for consistency
    }

}//Cls

//==========================================//
