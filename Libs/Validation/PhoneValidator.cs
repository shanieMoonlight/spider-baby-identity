using PhoneNumbers;
using StringHelpers;
using System.Text.RegularExpressions;

namespace ValidationHelpers;

/// <summary>
/// Validates strings as properly formatted phone numbers.
/// </summary>
/// <remarks>
/// This implementation provides basic phone number validation, including support for
/// common separators and extension notations. It's designed to be flexible while
/// enforcing reasonable constraints for most phone number formats.
/// </remarks>
public partial class PhoneValidator
{

    // Generate regex for validating characters in phone numbers, including + at the beginning for international format
    [GeneratedRegex(@"^(\+)?[0-9\s\-\.\(\)]+$")]
    private static partial Regex ValidPhoneCharsRegex();

    // Generate regex for checking if a phone number has at least one digit
    [GeneratedRegex(@"\d")]
    private static partial Regex HasDigitRegex();

    // Generate regex for detecting and extracting extensions
    [GeneratedRegex(@"(?:ext\.?|x)\s*\d+$", RegexOptions.IgnoreCase)]
    private static partial Regex ExtensionRegex();

    //-------------------------//


    public static bool IsValid(string? phone, bool allowNulls) =>
        IsValid(phone, minLength: 6, allowNulls);

    public static bool IsValid(string? phone) =>
        IsValid(phone, minLength: 6, allowNulls: false);



    public static bool IsValid(string? phone, int minLength) => 
        IsValid(phone, minLength, allowNulls: false);



    /// <summary>
    /// Validates if the provided string is a valid phone number.
    /// </summary>
    /// <param name="phone">The phone number to validate.</param>
    /// <param name="minLength">The minimum required length after processing.</param>
    /// <param name="allowNulls">Whether to consider null/empty values valid.</param>
    /// <returns>True if valid, false otherwise.</returns>
    public static bool IsValid(string? phone, int minLength, bool allowNulls)
    {

        // Trim the input but keep + sign for international format
        phone = phone?.RemoveWhitespaces();

        // Check for null or whitespace
        if (string.IsNullOrWhiteSpace(phone))
            return allowNulls;

        // Remove extension if present
        phone = ExtensionRegex().Replace(phone, string.Empty).TrimEnd();


        // Check minimum length
        if (phone.Length < minLength)
            return false;

        // Check that phone has at least one digit
        if (!HasDigitRegex().IsMatch(phone))
            return false;

        // Verify that phone only contains valid characters including possible + at beginning
        return ValidPhoneCharsRegex().IsMatch(phone);
    }


}//Cls