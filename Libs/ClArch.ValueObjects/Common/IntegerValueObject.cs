namespace ClArch.ValueObjects.Common;

//========================//


public abstract class IntegerValueObject(int value) : ValueObject<int>(value)
{
}//Cls


//========================//


public abstract class NullableIntegerValueObject(int? value) : ValueObject<int?>(value)
{
    protected override bool ValuesAreEqual(int? thatValue) =>
        Value == thatValue;

}//Cls


//========================//


