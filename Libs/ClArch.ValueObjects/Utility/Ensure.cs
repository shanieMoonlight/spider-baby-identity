using StringHelpers;
using ClArch.ValueObjects.Exceptions;
using System;

namespace ClArch.ValueObjects.Utility;
public static class Ensure
{
    public static void MaxLength(string? value, int maxLength, string propertyName)
    {
        if (value is not null && value.Length > maxLength)
            throw new StringTooLongPropertyException(propertyName, maxLength, value);
    }

    //- - - - - - - - - - - - - - - - - -//

    public static void MaxLengthTrimmed(string? value, int maxLength, string propertyName)
    {
        if (value is not null && value.Trim().Length > maxLength)
            throw new StringTooLongPropertyException(propertyName, maxLength, value);
    }

    //- - - - - - - - - - - - - - - - - -//

    public static void NotNullOrWhiteSpace(string? value, string propertyName)
    {
        if (value.IsNullOrWhiteSpace())
            throw new IsRequiredPropertyException(propertyName);
    }

    //- - - - - - - - - - - - - - - - - -//

    public static void ValidId(int? value, string propertyName)
    {
        if (!value.HasValue || value < 1)
            throw new InvalidIdPropertyException(propertyName);
    }

    //- - - - - - - - - - - - - - - - - -//

    public static void IsRequired(DateTime? value, string propertyName)
    {
        if (!value.HasValue || value == default)
            throw new IsRequiredPropertyException(propertyName);
    }

    //- - - - - - - - - - - - - - - - - -//

    public static void IsRequired<T>(T? value, string propertyName)
    {
        if (value is null)
            throw new IsRequiredPropertyException(propertyName);
    }

    //- - - - - - - - - - - - - - - - - -//

    public static void IsNotDefault<T>(T? value, string propertyName) where T : struct
    {
        if (value is null || EqualityComparer<T>.Default.Equals(value.Value, default))
            throw new IsRequiredPropertyException(propertyName);
    }

    //- - - - - - - - - - - - - - - - - -//

    public static void ValidRange<T>(T? value, T min, T max,  string propertyName) where T : IComparable, IComparable<T>
    {
        if (value is null || value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new OutOfRangePropertyException<T>(propertyName, min, max);
    }

    //- - - - - - - - - - - - - - - - - -//

    public static void MinValue<T>(T value, T min, string propertyName) where T : IComparable, IComparable<T>
    {
        if (value is null || value.CompareTo(min) < 0)
            throw new MinValuePropertyException<T>(propertyName, min);
    }

    //- - - - - - - - - - - - - - - - - -//

    public static void MaxValue<T>(T value, T max, string propertyName) where T : IComparable, IComparable<T>
    {
        if (value is null || value.CompareTo(max) > 0)
            throw new MaxValuePropertyException<T>(propertyName, max);
    }


}//Cls
