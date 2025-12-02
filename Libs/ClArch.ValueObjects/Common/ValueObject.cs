namespace ClArch.ValueObjects.Common;

public abstract class ValueObject<T>(T value) : IEquatable<ValueObject<T>>
{
    public T Value { get; } = value;

    //----------------------------//

    protected virtual bool ValuesAreEqual(T? thatValue) =>
       EqualityComparer<T>.Default.Equals(Value, thatValue);


    public bool Equals(ValueObject<T>? that) =>
        that is not null &&
        GetType() == that.GetType() &&
        ValuesAreEqual(that.Value);

    public override bool Equals(object? other) => 
        other is ValueObject<T> otherObject && 
        Equals(otherObject);

    //----------------------------//

    public override int GetHashCode() =>
        HashCode.Combine(GetType(), Value);

    //----------------------------//

    public static bool operator ==(ValueObject<T>? lhs, ValueObject<T>? rhs)
    {
        if (ReferenceEquals(lhs, rhs))
            return true;

        if (lhs is null || rhs is null)
            return false;

        // Equals handles case of null on right side.
        return lhs.Equals(rhs);
    }


    public static bool operator !=(ValueObject<T>? lhs, ValueObject<T>? rhs) =>
        !(lhs == rhs);


    //----------------------------//

    public override string ToString() =>
        Value?.ToString() ?? string.Empty;

   
}//Cls




