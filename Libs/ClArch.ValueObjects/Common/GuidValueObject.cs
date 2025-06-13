namespace ClArch.ValueObjects.Common;

//========================//

public abstract class GuidValueObject(Guid value) : ValueObject<Guid>(value)
{
}//Cls

//========================//

public abstract class NullableGuidValueObject(Guid? value) : ValueObject<Guid?>(value)
{
    protected override bool ValuesAreEqual(Guid? thatValue) =>
        Value == thatValue;

}//Cls

//========================//


