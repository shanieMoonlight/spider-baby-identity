using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;

namespace ClArch.ValueObjects;
public class FileName : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthFileName;
    private FileName(string value) : base(value) { }

    public static FileName Create(string value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(FileNameWithoutExtension));
        return new(value);
    }

    public static FileName Create(Guid value)
    {
        return new(value.ToString());
    }

}//Cls