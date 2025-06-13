namespace ClArch.ValueObjects.Exceptions;

//==========================================================//

public class InvalidPropertyException(string property, string message) : Exception(message)
{
    public string Property { get; set; } = property;
}//Cls

//==========================================================//

public class NegativeValuePropertyException : InvalidPropertyException
{
    public NegativeValuePropertyException(string property, int value)
        : base(property, $"{property}, {value} is invalid. {property} must be non-negative") =>
        Property = property;

}//Cls

//==========================================================//

public class StringTooLongPropertyException : InvalidPropertyException
{
    public StringTooLongPropertyException(string property, int maxLength, string receivedValue)
        : base(property, $"{property} must me less than {maxLength} characters long.\r\nReceived Value: {receivedValue}") =>
        Property = property;

}//Cls

//==========================================================//

public class IsRequiredPropertyException : InvalidPropertyException
{
    public IsRequiredPropertyException(string property)
        : base(property, $"{property} is required") =>
        Property = property;

}//Cls

//==========================================================//

public class InvalidIdPropertyException : InvalidPropertyException
{
    public InvalidIdPropertyException(string property)
        : base(property, $"{property} is not a valid ID") =>
        Property = property;

}//Cls

//==========================================================//

public class OutOfRangePropertyException<T> : InvalidPropertyException where T  : IComparable, IComparable<T>
{
    public OutOfRangePropertyException(string property, T min, T max)
        : base(property, $"{property} must be between {min} and {max}") =>
        Property = property;

}//Cls

//==========================================================//

public class MinValuePropertyException<T> : InvalidPropertyException where T : IComparable, IComparable<T>
{
    public MinValuePropertyException(string property, T min)
        : base(property, $"{property} must be greater than {min}") =>
        Property = property;

}//Cls

//==========================================================//

public class MaxValuePropertyException<T> : InvalidPropertyException where T : IComparable, IComparable<T>
{
    public MaxValuePropertyException(string property,T max)
        : base(property, $"{property} must be less than {max}") =>
        Property = property;

}//Cls

//==========================================================//