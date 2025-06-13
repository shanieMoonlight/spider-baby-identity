using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;

namespace ClArch.ValueObjects;
public class ConfirmPassword : StringValueObject //Not using invariant value object for passwords
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthPwd;

    private ConfirmPassword(string value) : base(value) { }

    public static ConfirmPassword Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(ConfirmPassword));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(ConfirmPassword));


        return new(value);
    }

}//Cls