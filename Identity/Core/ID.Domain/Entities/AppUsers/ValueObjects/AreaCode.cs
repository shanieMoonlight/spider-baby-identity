using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ID.Domain.Entities.AppUsers.ValueObjects;


//=============================================================================//

public class AreaCode : StringValueObject
{
    public readonly static int MaxLength = 10;

    private AreaCode(string value) : base(value) { }

    public static AreaCode Create(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(AreaCode));
        Ensure.MaxLength(value, MaxLength, nameof(AreaCode));

        return new(value);
    }

}//Cls

//=============================================================================//

public class AreaCodeNullable : NullableStringValueObject
{
    public readonly static int MaxLength = 10;

    private AreaCodeNullable(string? value) : base(value) { }

    public static AreaCodeNullable Create(string? value)
    {
        Ensure.MaxLength(value, MaxLength, nameof(AreaCode));

        return new(value);
    }

}//Cls


//=============================================================================//


