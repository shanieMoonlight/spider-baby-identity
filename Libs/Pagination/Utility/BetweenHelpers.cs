using System.Linq.Expressions;

namespace Pagination.Utility;

public class BetweenHelpers
{

    //-----------------------------//

    /// <summary>
    /// Strings use d to separate 2 values in a field value that has a corresponding field type of between  
    /// </summary>
    public static readonly HashSet<string> BETWEEN_SEPARATORS =
        [
           "to",
         "-to-",
         "--to--"
        ];

    //-----------------------------//

    /// <summary>
    /// Attempts to split the value into (hopefully) 2 values of type string (start & end)
    /// </summary>
    /// <param name="filterRequest">The request containing the between value</param>
    /// <returns>An array containing the start & end values</returns>
    public static string[] SplitBetweenValue(FilterRequest filterRequest) =>
        filterRequest.FilterValue.Split(BETWEEN_SEPARATORS.ToArray(), StringSplitOptions.None);

    //-----------------------------//

    public static bool IsValidBetweenArray(string[] dateStrings) => 
        dateStrings.Length == 2;        // we should have 2 dateStrings after splitting on the betweenSeparator

    //-----------------------------//

    /// <summary>
    /// Checks whether this value can be parsed into a start value and an end value
    /// </summary>
    /// <param name="filterValue">The value to be parsed</param>
    /// <returns></returns>
    public static bool IsValidBetweenValue(string filterValue)
    {
        if (string.IsNullOrWhiteSpace(filterValue))
            return false;

        foreach (var separator in BETWEEN_SEPARATORS)
        {
            if (filterValue.Contains(separator.Trim()))
                return true;
        }

        return false;
    }

    //-----------------------------//

    /// <summary>
    /// Generates a between expression on <paramref name="property"/>
    /// </summary>
    /// <typeparam name="V">Type of <paramref name="property"/></typeparam>
    /// <param name="property">The field/property that should be between <paramref name="start"/> and <paramref name="end"/></param>
    /// <param name="start"><paramref name="property"/> must be greater than (or equal to) this value</param>
    /// <param name="end"><paramref name="property"/> must be less (or equal to) this value</param>
    /// <param name="inclusive">Set to false when <paramref name="property"/> shouold be strictly between <paramref name="start"/> and <paramref name="end"/></param>
    /// <returns>BetweenExpression</returns>
    public static Expression BetweenExpression<V>(MemberExpression property, V start, V end, bool inclusive = true)
    {

        var startValue = Expression.Constant(start);
        var endValue = Expression.Constant(end);

        return BetweenExpression(property, startValue, endValue, inclusive);

    }

    //-----------------------------//

    /// <summary>
    /// Generates a between expression on <paramref name="property"/>
    /// </summary>
    /// <param name="property">The field/property that should be between <paramref name="startValue"/> and <paramref name="endValue"/></param>
    /// <param name="startValue"><paramref name="property"/> must be greater than (or equal to) this value</param>
    /// <param name="endValue"><paramref name="property"/> must be less (or equal to) this value</param>
    /// <param name="inclusive">Set to false when <paramref name="property"/> shouold be strictly between <paramref name="startValue"/> and <paramref name="endValue"/></param>
    /// <returns></returns>
    public static Expression BetweenExpression(MemberExpression property, ConstantExpression startValue, ConstantExpression endValue, bool inclusive = true)
    {
        var startType = inclusive ? ExpressionType.GreaterThanOrEqual : ExpressionType.GreaterThan;
        var endType = inclusive ? ExpressionType.LessThanOrEqual : ExpressionType.LessThan;

        var startBinary = Expression.MakeBinary(startType, property, startValue);
        var endBinary = Expression.MakeBinary(endType, property, endValue);

        return Expression.MakeBinary(ExpressionType.AndAlso, startBinary, endBinary);

    }

    //-----------------------------//

}//Cls
