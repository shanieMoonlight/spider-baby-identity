using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;
using StringHelpers;

namespace ClArch.ValueObjects;

//==========================================//

public class ShortNotes : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthShortNotesContent;

    private ShortNotes(string value) : base(value) { }

    public static ShortNotes Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(ShortNotes));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(ShortNotes));

        return new(value);
    }

}//Cls

//==========================================//

public class ShortNotesNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthShortNotesContent;

    private ShortNotesNullable(string? value) : base(value) { }

    public static ShortNotesNullable Create(string? value)
    {
        if (value is not null)
            Ensure.MaxLengthTrimmed(value, MaxLength, nameof(ShortNotes));

        return new(value.IsNullOrWhiteSpace() ? null : value); //Don't store white spaces
    }

}//Cls

//==========================================//

public class ShortTruncateNotes : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthShortNotesContent;

    private ShortTruncateNotes(string value) : base(value) { }

    public static ShortTruncateNotes Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(ShortNotes));
        return new(value.Truncate(MaxLength) ?? "");
    }

}//Cls

//= = = = = = = = = = = = = = = = = = = = = //

public class ShortTruncateNotesNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthShortNotesContent;

    private ShortTruncateNotesNullable(string? value) : base(value) { }

    public static ShortTruncateNotesNullable Create(string? value)
        => new(value.IsNullOrWhiteSpace() ? null : value.Truncate(MaxLength));

}//Cls

//==========================================//