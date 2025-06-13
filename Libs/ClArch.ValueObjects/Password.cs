using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;

namespace ClArch.ValueObjects;
public class Password : StringValueObject  //Not using invariant value object for passwords
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthPwd;

    private Password(string value) : base(value) { }

    public static Password Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(Password));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(Password));


        return new(value);
    }

}//Cls