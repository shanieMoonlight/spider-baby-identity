using System.Text.RegularExpressions;

namespace StringHelpers;

/// <summary>
/// Class for helping with matching strings
/// </summary>
public partial class ParsingHelpers
{

    // Define the source-generated methods
    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespacesRegex();

    [GeneratedRegex("[^0-9]", RegexOptions.Compiled)]
    private static partial Regex NonDigitsRegex();

    [GeneratedRegex(@"[()-]+", RegexOptions.Compiled)]
    private static partial Regex PhoneNonDigitRegex();

    //-------------------------------//

    /// <summary>
    /// Does what it says on the tin.
    /// </summary>
    /// <param name="strOriginal"></param>
    /// <returns></returns>
    public static string RemoveWhitespaces(string strOriginal)
    {
        if (strOriginal == null)
            return "";

        return WhitespacesRegex().Replace(strOriginal, "");
    }

     //- - - - - - - - - - - - - - - -//

    public static string CleanEmail(string email) =>
        RemoveWhitespaces(email);

     //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Does what it says on the tin.
    /// </summary>
    /// <param name="strOriginal"></param>
    /// <returns>Digit only string</returns>
    public static string RemoveAllNonDigits(string strOriginal) =>
        NonDigitsRegex().Replace(strOriginal, "");

     //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks whether string contains x digits and nothing else.
    /// </summary>
    /// <param name="strOriginal"></param>
    /// <param name="numDigits"></param>
    /// <returns></returns>
    public static bool IsExactly_X_Digits(string strOriginal, int numDigits)
    {
        if (strOriginal == null)
            return false;


        Regex rgx_X_Digits = new(@"^[0-9]{" + numDigits + "}$");

        return rgx_X_Digits.IsMatch(strOriginal);

    }

     //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Removes whitespaces, '(',  ')' and '-'
    /// </summary>
    /// <param name="strOriginal"></param>
    /// <returns></returns>
    public static string CleanPhoneNumber(string strOriginal)
    {
        if (strOriginal == null)
            return "";

        string cleanedString = WhitespacesRegex().Replace(strOriginal, "");

        //Remove other bits
        cleanedString = PhoneNonDigitRegex().Replace(cleanedString, "");

        return cleanedString;
    }

     //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Removes single/double quotes if the string is surrounded by them.
    /// </summary>
    /// <param name="strOriginal">String to adjust</param>
    /// <returns>Input string without quotes</returns>
    public static string RemoveQuotes(string strOriginal)
    {
        if (strOriginal == null)
            return "";

        string cleanedString = strOriginal.Trim();

        if (cleanedString.StartsWith('"') && cleanedString.EndsWith('"'))
        {
            cleanedString = cleanedString[1..^1];
            return RemoveQuotes(cleanedString);
        }


        if (cleanedString.StartsWith('\'') && cleanedString.EndsWith('\''))
        {
            cleanedString = cleanedString[1..^1];
            return RemoveQuotes(cleanedString);
        }

        return cleanedString;

    }



}//Cls
