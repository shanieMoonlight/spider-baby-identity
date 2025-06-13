using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Utility;

namespace ClArch.ValueObjects;
public class RegistrationDate : DateValueObject
{
    private RegistrationDate(DateTime value) : base(value) { }

    public static RegistrationDate Create(DateTime value)
    {
        Ensure.IsNotDefault<DateTime>(value, nameof(RegistrationDate));
        return new(value);
    }
}//Rcd

//========================//

public class RegistrationDateNullable : NullableDateValueObject
{
    private RegistrationDateNullable(DateTime? value) : base(value) { }

    public static RegistrationDateNullable Create(DateTime? value = null) =>
        value == default(DateTime)
                ? new(null) //Don't set a value if default
                : new(value);

}//Rcd

//========================//
