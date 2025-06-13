using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ClArch.ValueObjects;
public class DateOfBirth : DateValueObject
{
    private DateOfBirth(DateTime value) : base(value) { }

    public static DateOfBirth Create(DateTime value)
    {
        Ensure.IsNotDefault<DateTime>(value, nameof(DateOfBirth));
        return new(value);
    }

}//Rcd

//===========================================================//

public class DateOfBirthNullable : NullableDateValueObject
{
    private DateOfBirthNullable(DateTime? value) : base(value) { }

    public static DateOfBirthNullable Create(DateTime? value = null) =>
        value == default(DateTime)
                ? new(null) //Don't set a value if default
                : new(value);

}//Rcd
