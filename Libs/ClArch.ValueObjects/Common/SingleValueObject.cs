namespace ClArch.ValueObjects.Common;

public abstract class SingleValueObject
{
    public abstract IEnumerable<object> GetAtomicValues();

    //----------------------------------------//

    public bool Equals(SingleValueObject? other) =>
        other is not null && ValuesAreEqual(other);

    //- - - - - - - - - - - - - - - - - - - - //

    public override bool Equals(object? other) =>
        other is SingleValueObject @otherObject && ValuesAreEqual(@otherObject);

    //----------------------------------------//

    private bool ValuesAreEqual(SingleValueObject other) =>
        GetAtomicValues().SequenceEqual(other.GetAtomicValues());

    //----------------------------------------//

    public override int GetHashCode() =>
        GetAtomicValues()
            .Aggregate(default(int), HashCode.Combine);


}//Cls


