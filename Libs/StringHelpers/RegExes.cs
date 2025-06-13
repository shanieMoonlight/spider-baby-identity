namespace StringHelpers;

public class RegExes
{
    public const string NON_ALPHA_NUMERIC = @"[^\p{L}\p{N}]+";
    public const string NON_ALPHA_NUMERIC_OR_WHITESPACE = @"[^\s|\p{L}\p{N}]+";
    public const string HEX_COLOR = @"^#(?:[0-9a-fA-F]{3}){1,2}$";
    public const string WHITESPACE_ALL = @"\s+";
    public const string WHITESPACE_HORIZONTAL = @"[^\S\r\n]";


}//Cls
