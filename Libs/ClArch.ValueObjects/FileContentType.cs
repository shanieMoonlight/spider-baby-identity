using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;

namespace ClArch.ValueObjects;
public class FileContentType : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthFileName;

    private FileContentType(string value) : base(value) { }

    public static FileContentType Create(string? value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(FileContentType));
        return new(value ?? ValueObjectsDefaultValues.FILE_CONTENT_TYPE);
    }

    public static FileContentType Unknown()
    {
        return new(ValueObjectsDefaultValues.FILE_CONTENT_TYPE);
    }
}//Cls