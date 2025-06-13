using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;

namespace ClArch.ValueObjects;
public class Name : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthName;

    private Name(string value) : base(value) { }

    public static Name Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(Name));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(Name));

        return new(value);
    }

}//Cls




