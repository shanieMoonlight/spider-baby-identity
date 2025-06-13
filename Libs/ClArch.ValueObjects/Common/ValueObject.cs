namespace ClArch.ValueObjects.Common;

public abstract class ValueObject<T>(T value)
{
    public T Value = value;

    //----------------------------------------//

    protected virtual bool ValuesAreEqual(T thatValue) =>
        Value?.Equals(thatValue) ?? false;

    //----------------------------------------//

    public bool Equals(ValueObject<T> that) =>
        that is not null && ValuesAreEqual(that.Value);

    //- - - - - - - - - - - - - - - - - - - - //

    public override bool Equals(object? other) =>
        other is ValueObject<T> @otherObject && ValuesAreEqual(@otherObject.Value);

    //- - - - - - - - - - - - - - - - - - - - //

    public override int GetHashCode() =>
        Value?.GetHashCode() ?? 0;

    //----------------------------------------//

    public static bool operator ==(ValueObject<T> lhs, ValueObject<T> rhs)
    {
        if (lhs is null)
            return rhs is null;

        // Equals handles case of null on right side.
        return lhs.Equals(rhs);
    }

    //- - - - - - - - - - - - - - - - - - - - //

    public static bool operator !=(ValueObject<T> lhs, ValueObject<T> rhs) =>
        !(lhs == rhs);

    //----------------------------------------//

    public override string ToString() => 
        Value?.ToString() ?? string.Empty;

    //----------------------------------------//

}//Cls




