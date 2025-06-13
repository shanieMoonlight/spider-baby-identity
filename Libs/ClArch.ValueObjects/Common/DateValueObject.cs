namespace ClArch.ValueObjects.Common;

//========================//


public abstract class DateValueObject(DateTime value) : ValueObject<DateTime>(value)
{
}//Cls


//========================//


public abstract class NullableDateValueObject(DateTime? value) : ValueObject<DateTime?>(value)
{
    protected override bool ValuesAreEqual(DateTime? thatValue) =>
        Value == thatValue;

}//Cls


//========================//


