using ClArch.ValueObjects.Common;

namespace ClArch.ValueObjects;

//========================//

public class Amount : DoubleValueObject
{
    public const double MinAmount = 0;

    private Amount(double value) : base(value) { }

    public static Amount Create(double value) =>
        new(Math.Max(value, MinAmount));

}//Cls

//= = = = = = = = = = = = //


public class AmountNullable : NullableDoubleValueObject
{
    public const double MinAmount = 0;

    private AmountNullable(double? value) : base(value) { }

    public static AmountNullable Create(double? value)
    {
        if (value is not null)
            value = Math.Max(value.Value, MinAmount);
        return new(value);
    }
}//Cls

//========================//


public class AmountInteger : IntegerValueObject
{
    public const int MinAmount = 0;

    private AmountInteger(int value) : base(value) { }

    public static AmountInteger Create(int value) =>
        new(Math.Max(value, MinAmount));

}//Cls

//= = = = = = = = = = = = //

public class AmountIntegerNullable : NullableIntegerValueObject
{
    public const int MinAmount = 0;

    private AmountIntegerNullable(int? value) : base(value) { }

    public static AmountIntegerNullable Create(int? value)
    {
        if (value is not null)
            value = Math.Max(value.Value, MinAmount);
        return new(value);
    }
}//Cls

//========================//
