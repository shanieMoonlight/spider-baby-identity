using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;
using StringHelpers;

namespace ClArch.ValueObjects;

public class ImgUrl : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthResourceUrl;

    private ImgUrl(string value) : base(value) { }

    public static ImgUrl Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(ImgUrl));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(ImgUrlNullable));

        return new(value);
    }

}//Cls




public class ImgUrlNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthResourceUrl;

    private ImgUrlNullable(string? value) : base(value) { }

    public static ImgUrlNullable Create(string? value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(ImgUrlNullable));

        return new(value.IsNullOrWhiteSpace() ? null : value);
    }

}//Cls


