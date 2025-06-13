using Microsoft.Extensions.Primitives;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StringHelpers;

public static partial class StringExtensions
{
    private static readonly CultureInfo _currentCultureInfo = CultureInfo.InvariantCulture;


    [GeneratedRegex(RegExes.NON_ALPHA_NUMERIC)]
    private static partial Regex NonAlphaNumericRegex();

    [GeneratedRegex(RegExes.NON_ALPHA_NUMERIC_OR_WHITESPACE)]
    private static partial Regex NonAlphaNumericOrWhitespaceRegex();

    [GeneratedRegex(RegExes.WHITESPACE_ALL)]
    private static partial Regex WhitespacePatternRegex();



    //-------------------------------//

    /// <summary>
    /// Appends <paramref name="path"/> to <paramref name="baseUrl"/>
    /// </summary>
    public static string AppendPath(this string baseUrl, string path)
    {

        Uri baseUri = new(baseUrl);
        Uri myUri = new(baseUri, path);
        return myUri.ToString();

    }

    //- - - - - - - - - - - - - - - -//

    public static string CamelToPascal(this string str) =>
        string.IsNullOrWhiteSpace(str) ? str : str.First().ToString().ToUpper() + str[1..];

    //- - - - - - - - - - - - - - - -//

    public static string CamelToTitleCase(this string titleCaseStr)
    {

        if (titleCaseStr.IsNullOrWhiteSpace())
            return titleCaseStr;

        titleCaseStr = titleCaseStr.CamelToPascal();

        string[] spacedWords = titleCaseStr
       .Select(c => c == char.ToUpper(c, _currentCultureInfo) ? " " + c.ToString() : c.ToString())
       .ToArray();

        return string.Join("", spacedWords).Trim();
    }

    //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Gets the hostname from an url
    /// </summary>
    /// <param name="url">Website url</param>
    /// <returns>The hostname or null</returns>
    public static string GetHost(this string url) =>
        url.IsNullOrWhiteSpace() ? url : new Uri(url).Host;

    //- - - - - - - - - - - - - - - -//

    public static bool InvariantEquals(this string thisStr, string thatStr)
    {
        //both null means they are equal
        if (thisStr == null && thatStr == null)
            return true;
        //only one is null here
        if (thisStr == null || thatStr == null)
            return false;

        return thisStr.ToLower() == thatStr.ToLower();

    }

    //- - - - - - - - - - - - - - - -//

    public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);

    //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// True if list is null or list is empty or all elements are null or white space
    /// </summary>
    /// <param name="strList"></param>
    /// <returns></returns>
    public static bool IsNullOrWhiteSpace(this IEnumerable<string> strList) =>
        strList == null || !strList.Any() || !strList.Any(s => !s.IsNullOrWhiteSpace());

    //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// removers everything except letters and numbers
    /// </summary>
    /// <param name="strOriginal">Original string</param>
    /// <param name="toLower">return a lowercase version</param>
    /// <returns></returns>
    public static string OnlyAlphanumeric(this string? strOriginal, bool toLower = false)
    {
        if (strOriginal == null)
            return "";

        var strOnlyAlpha = NonAlphaNumericRegex().Replace(strOriginal, "");
        return toLower ? strOnlyAlpha.ToLower() : strOnlyAlpha;

    }

    //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// removers everything except letters and numbers
    /// </summary>
    /// <param name="strOriginal">Original string</param>
    /// <param name="toLower">return a lowercase version</param>
    /// <returns></returns>
    public static string OnlyAlphanumericOrWhiteSpace(this string? strOriginal, bool toLower = false)
    {
        if (strOriginal == null)
            return "";

        var strOnlyAlpha = NonAlphaNumericOrWhitespaceRegex().Replace(strOriginal, "");
        return toLower ? strOnlyAlpha.ToLower() : strOnlyAlpha;

    }

    //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Make sure the filename is in the right format with the right extension
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="ext">The extension that the file should have</param>
    /// <param name="toLower">Convert name to lowercase</param>
    /// <returns></returns>
    public static string SanitizeFilename(this string? filename, string ext, bool toLower = true)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return filename ?? "";

        if (!filename.EndsWith(ext))
            filename = $"{filename}{ext}";

        if (toLower)
            return filename.ToLower();

        return filename;

    }

    //- - - - - - - - - - - - - - - -//

    public static string SentenceToTitleCase(this string? sentence)
    {
        if (sentence.IsNullOrWhiteSpace())
            return sentence ?? "";

        TextInfo cultInfo = _currentCultureInfo.TextInfo;
        return cultInfo.ToTitleCase(sentence!).Trim();

    }

    //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Does what it says on the tin.
    /// </summary>
    /// <param name="strOriginal"></param>
    /// <param name="toLower">return a lowercase version</param>
    /// <returns></returns>
    public static string RemoveWhitespaces(this string? strOriginal, bool toLower = false)
    {
        if (strOriginal == null)
            return string.Empty;

        var strNoWhitespace = WhitespacePatternRegex().Replace(strOriginal, "");

        if (toLower)
            return strNoWhitespace.ToLower();
        else
            return strNoWhitespace;


    }

    //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Returns <paramref name="chars"/> characters form the end of the string or the whole string if it's <=  <paramref name="chars"/>
    /// </summary>
    /// <param name="str"></param>
    /// <param name="chars"></param>
    /// <returns>The last one</returns>
    public static string TakeLast(this string str, int chars)
    {
        if (str == null)
            return "";
        else if (str.Length >= chars)
            return str.Substring(str.Length - chars, chars);
        else
            return str;

    }

    //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Turns <paramref name="doubleStr"/> into a double - falls back to <paramref name="defaultVal"/>
    /// </summary>
    /// <param name="doubleStr"></param>
    /// <param name="defaultVal"></param>
    /// <returns>Double version of string or <paramref name="defaultVal"/></returns>
    public static double ToDouble(this string doubleStr, double defaultVal = -1)
    {
        if (string.IsNullOrWhiteSpace(doubleStr))
            return defaultVal;

        bool parseSucceeded = double.TryParse(doubleStr, out double outVal);

        return parseSucceeded ? outVal : defaultVal;

    }

    //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Turns <paramref name="intStr"/> into an int - falls back to <paramref name="defaultVal"/>
    /// </summary>
    /// <param name="intStr"></param>
    /// <param name="defaultVal"></param>
    /// <returns>Int version of string or <paramref name="defaultVal"/></returns>
    public static int ToInt(this string intStr, int defaultVal = -1)
    {
        if (string.IsNullOrWhiteSpace(intStr))
            return defaultVal;

        bool parseSucceeded = int.TryParse(intStr, out int outVal);

        return parseSucceeded ? outVal : defaultVal;

    }

    //- - - - - - - - - - - - - - - -//

    public static string Capitalize(this string? str) =>
        str == null ? "" : char.ToUpper(str[0]) + str[1..];


    //- - - - - - - - - - - - - - - -//


    public static string ToTitleCase(this string? str) =>
        str == null ? "" : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());


    //- - - - - - - - - - - - - - - -//


    public static string ToCamelCase(this string? str) =>
        str == null ? "" : char.ToLowerInvariant(str[0]) + str[1..];


    //- - - - - - - - - - - - - - - -//


    /// <summary>
    /// Replaces any groups of multiple whitespaces with single space
    /// </summary>
    /// <param name="str"></param>
    /// <returns>Altered string</returns>
    public static string TrimAndSingleSpaced(this string str, bool keepNewLine = false)
    {
        var regExPattern = keepNewLine ? RegExes.WHITESPACE_HORIZONTAL : RegExes.WHITESPACE_ALL;
        return str == null ? "" : Regex.Replace(str, regExPattern, " ").Trim();

    }

    //- - - - - - - - - - - - - - - -//

    /// <summary>
    /// Truncates this <paramref name="str"/> to <paramref name="maxChars"/> and adds ellipses if 
    /// returned values is shorter than entered value
    /// </summary>
    /// <param name="str"></param>
    /// <param name="maxChars"></param>
    /// <returns></returns>
    public static string? Truncate(this string? str, int maxChars) =>
        string.IsNullOrWhiteSpace(str) || str.Length <= maxChars
        ? str
        : $"{str[..(maxChars - 3)]}..."; // - 3 for the dots


    //- - - - - - - - - - - - - - - -//


    public static string RemovePrefix(this string text, string prefix) =>
        text.StartsWith(prefix) ? text[prefix.Length..] : text;


    //- - - - - - - - - - - - - - - -//


    public static string ReplaceFirst(this string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
            return text;

        return string.Concat(text.AsSpan(0, pos), replace, text.AsSpan(pos + search.Length));

    }

    //- - - - - - - - - - - - - - - -//


    /// <summary>
    /// Replace all Whitespaces with <paramref name="replacement"/>
    /// </summary>
    /// <param name="text">Text to alter (The one with potential whitespaces)</param>
    /// <param name="replacement">The thing to put in their place</param>
    /// <returns>Updated string</returns>
    public static string ReplaceWhitespaces(this string text, string replacement = "_") =>
        text == "" ? text : WhitespacePatternRegex().Replace(text, replacement);


    //- - - - - - - - - - - - - - - -//


    public static string RemoveFirst(this string text, string search)
        => text.ReplaceFirst(search, "");


    //- - - - - - - - - - - - - - - -//


    public static string ReverseBackSlashes(this string text)
        => text.Replace("\\", "/");


    //- - - - - - - - - - - - - - - -//


    /// <summary>
    ///Converts <paramref name="dateStr"/> to date.
    ///Return new Datetime if it fails
    /// </summary>
    /// <param name="dateStr">Dat in string form</param>
    /// <returns>DateTime</returns>        
    public static DateTime ToDate(this StringValues dateStr)
    {
        try
        {
            return Convert.ToDateTime(dateStr);
        }
        catch (Exception)
        {
            return new DateTime();
        }

    }


    //- - - - - - - - - - - - - - - -//


    /// <summary>
    /// Replaces multiple consecutive spaces with single space
    /// </summary>
    public static string OnlySingleSpaces(this string text) => WhitespacePatternRegex().Replace(text, " ");


    //- - - - - - - - - - - - - - - -//


    public static string EnsureLeadingChar(this string text, char leadingChar)
       => (text!.StartsWith(leadingChar) ? text : $"{leadingChar}{text}");


}//Cls
