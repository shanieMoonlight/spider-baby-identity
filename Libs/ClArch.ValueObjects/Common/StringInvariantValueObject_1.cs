namespace ClArch.ValueObjects.Common;

//========================//

/// <summary>
/// Base class for value objects that wrap a non-nullable string.
/// </summary>
/// <remarks>
/// <para>
/// This abstract class implements common behaviors for string-based value objects:
/// - Automatically trims input strings to remove whitespace
/// - Provides case-insensitive equality comparison
/// - Implements value semantics through the ValueObject base class
/// </para>
/// <para>
/// Use this class when you need a strongly-typed wrapper around a string value
/// that must not be null.
/// </para>
/// </remarks>
/// <param name="value">The string value to encapsulate (will be trimmed)</param>
public abstract class StringInvariantValueObject(string value) : ValueObject<string>(value.Trim())
{
    /// <summary>
    /// Determines whether this value object's value equals another string value.
    /// </summary>
    /// <param name="thatValue">The string value to compare against.</param>
    /// <returns>True if the values are equal (case-insensitive), false otherwise.</returns>
    protected override bool ValuesAreEqual(string thatValue) =>
        Value.Equals(thatValue, StringComparison.CurrentCultureIgnoreCase);

}//Cls

//========================//

/// <summary>
/// Base class for value objects that wrap a nullable string.
/// </summary>
/// <remarks>
/// <para>
/// This abstract class implements common behaviors for nullable string-based value objects:
/// - Automatically trims non-null input strings to remove whitespace
/// - Provides case-insensitive equality comparison that handles null values
/// - Implements value semantics through the ValueObject base class
/// </para>
/// <para>
/// Use this class when you need a strongly-typed wrapper around a string value
/// that may be null.
/// </para>
/// </remarks>
/// <param name="value">The nullable string value to encapsulate (will be trimmed if not null)</param>
public abstract class NullableStringInvariantValueObject(string? value) : ValueObject<string?>(value?.Trim())
{
    /// <summary>
    /// Determines whether this value object's value equals another string value.
    /// </summary>
    /// <param name="thatValue">The nullable string value to compare against.</param>
    /// <returns>
    /// True if both values are null or both are non-null and equal (case-insensitive), 
    /// false otherwise.
    /// </returns>
    protected override bool ValuesAreEqual(string? thatValue) =>
        (Value is null && thatValue is null) ||
        (Value is not null && thatValue is not null &&
         string.Equals(Value, thatValue, StringComparison.CurrentCultureIgnoreCase));

}//Cls

//========================//