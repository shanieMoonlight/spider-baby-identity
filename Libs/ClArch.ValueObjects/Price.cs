using ClArch.ValueObjects.Common;

namespace ClArch.ValueObjects;

//========================//

public class Price : DoubleValueObject
{
    public const double MinPrice = 0;

    private Price(double value) : base(value) { }

    public static Price Create(double value) =>
        new(Math.Max(value, MinPrice));

}//Cls

//= = = = = = = = = = = = //

public class PriceNullable : NullableDoubleValueObject
{
    public const double MinPrice = 0;

    private PriceNullable(double? value) : base(value) { }

    public static PriceNullable Create(double? value)
    {
        if (value is not null)
            value = Math.Max(value.Value, MinPrice);
        return new(value);
    }
}//Cls

//========================//


public class PriceInteger : IntegerValueObject
{
    public const int MinPrice = 0;

    private PriceInteger(int value) : base(value) { }

    public static PriceInteger Create(int value) =>
        new(Math.Max(value, MinPrice));

}//Cls

//= = = = = = = = = = = = //

public class PriceIntegerNullable : NullableIntegerValueObject
{
    public const int MinPrice = 0;

    private PriceIntegerNullable(int? value) : base(value) { }

    public static PriceIntegerNullable Create(int? value)
    {
        if (value is not null)
            value = Math.Max(value.Value, MinPrice);
        return new(value);
    }
}//Cls

//========================//
