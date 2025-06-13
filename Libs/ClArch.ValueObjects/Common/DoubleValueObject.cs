namespace ClArch.ValueObjects.Common;

//========================//


public abstract class DoubleValueObject(double value) : ValueObject<double>(value)
{

}//Cls


//========================//


public abstract class NullableDoubleValueObject(double? value) : ValueObject<double?>(value)
{
    protected override bool ValuesAreEqual(double? thatValue) =>
        Value == thatValue;

}//Cls


//========================//


