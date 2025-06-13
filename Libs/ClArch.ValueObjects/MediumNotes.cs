using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;
using StringHelpers;

namespace ClArch.ValueObjects;

//==========================================//

public class MediumNotes : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthMediumNotesContent;

    private MediumNotes(string value) : base(value) { }

    public static MediumNotes Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(MediumNotes));
        Ensure.MaxLengthTrimmed(value, MaxLength, nameof(MediumNotes));

        return new(value);
    }

}//Cls

//= = = = = = = = = = = = = = = = = = = = = //

public class MediumNotesNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthMediumNotesContent;

    private MediumNotesNullable(string? value) : base(value) { }

    public static MediumNotesNullable Create(string? value)
    {
        if (!value.IsNullOrWhiteSpace())
            Ensure.MaxLengthTrimmed(value, MaxLength, nameof(MediumNotes));

        return new(value.IsNullOrWhiteSpace() ? null : value); //Don't store white spaces
    }

}//Cls

//==========================================//

public class MediumTruncateNotes : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthMediumNotesContent;

    private MediumTruncateNotes(string value) : base(value) { }

    public static MediumTruncateNotes Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(MediumNotes));
        return new(value.Truncate(MaxLength) ?? "");
    }

}//Cls

//= = = = = = = = = = = = = = = = = = = = = //

public class MediumTruncateNotesNullable : NullableStringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthMediumNotesContent;

    private MediumTruncateNotesNullable(string? value) : base(value) { }

    public static MediumTruncateNotesNullable Create(string? value) 
        => new(value.IsNullOrWhiteSpace() ? null : value.Truncate(MaxLength));

}//Cls

//==========================================//