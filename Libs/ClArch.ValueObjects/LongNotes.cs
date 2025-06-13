using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;
using StringHelpers;

namespace ClArch.ValueObjects;

//==========================================//

public class LongNotes : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthLongNotesContent;

    private LongNotes(string value) : base(value) { }

    public static LongNotes Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(LongNotes));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(LongNotes));

        return new(value);
    }

}//Cls

//= = = = = = = = = = = = = = = = = = = = = //

public class LongNotesNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthLongNotesContent;

    private LongNotesNullable(string? value) : base(value) { }

    public static LongNotesNullable Create(string? value)
    {
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(LongNotes));

        return new(value.IsNullOrWhiteSpace() ? null : value); //Don't store white spaces
    }

}//Cls

//==========================================//

public class LongTruncateNotes : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthLongNotesContent;

    private LongTruncateNotes(string value) : base(value) { }

    public static LongTruncateNotes Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(LongNotes));
        return new(value.Truncate(MaxLength) ?? "");
    }

}//Cls

//= = = = = = = = = = = = = = = = = = = = = //

public class LongTruncateNotesNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthLongNotesContent;

    private LongTruncateNotesNullable(string? value) : base(value) { }

    public static LongTruncateNotesNullable Create(string? value) =>
        new(value.IsNullOrWhiteSpace() ? null : value.Truncate(MaxLength));
}//Cls

//==========================================//