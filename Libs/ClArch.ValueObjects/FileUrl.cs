using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;
using StringHelpers;

namespace ClArch.ValueObjects;

public class FileUrl : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthResourceUrl;

    private FileUrl(string value) : base(value) { }

    public static FileUrl Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(FileUrl));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(FileUrlNullable));

        return new(value);
    }

}//Cls




public class FileUrlNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthResourceUrl;

    private FileUrlNullable(string? value) : base(value) { }

    public static FileUrlNullable Create(string? value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(FileUrlNullable));
        return new(value.IsNullOrWhiteSpace() ? null: value);
    }

}//Cls


