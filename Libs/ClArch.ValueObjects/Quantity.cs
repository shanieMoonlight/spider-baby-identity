using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Exceptions;

namespace ClArch.ValueObjects;

//====================================================//

public class Quantity : ValueObject<int>
{
    private Quantity(int value) : base(value) { }

    public static Quantity Create(int value)
    {
        if (value < 0)
            throw new InvalidPropertyException(nameof(Quantity), $"The {nameof(Quantity)}: {value} is not valid.");

        return new(value);
    }

}//Cls

//====================================================//

public class QuantityNullable : ValueObject<int?>
{
    private QuantityNullable(int? value) : base(value) { }

    public static QuantityNullable Create(int? value)
    {
        if (value.HasValue && value.Value < 0)
            throw new InvalidPropertyException(nameof(Quantity), $"The {nameof(Quantity)}: {value} is not valid.");

        return new(value);
    }

}//Cls

//====================================================//