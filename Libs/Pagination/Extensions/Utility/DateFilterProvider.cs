using Pagination.Result;
using Pagination.Utility;
using System.Linq.Expressions;

namespace Pagination.Extensions.Utility;
/// <summary>
/// Provides functionality to create date-based filter expressions for LINQ queries.
/// </summary>
/// <remarks>
/// This class handles both standard date comparison operations and special cases like BETWEEN.
/// All date comparisons are normalized to date-only (ignoring time components).
/// </remarks>
internal class DateFilterProvider
{
    
    /// <summary>
    /// Creates a date filter expression based on the provided filter request.
    /// </summary>
    /// <typeparam name="T">The type of objects being filtered</typeparam>
    /// <param name="param">The parameter expression (typically "x => ") to build upon</param>
    /// <param name="filterRequest">The filter request containing field, filter type, and filter value</param>
    /// <param name="getPropertySelectorLambda">Optional function to transform property names (useful for mapping frontend field names to actual property names)</param>
    /// <returns>A result containing either the filter expression or an error message</returns>
    /// <remarks>
    /// This method:
    /// - Handles special cases like BETWEEN ranges
    /// - Normalizes all DateTime values to date-only (time component set to midnight)
    /// - Properly handles nullable DateTime properties
    /// - Returns appropriate error messages for parsing failures
    /// </remarks>
    internal static PgResult<Expression> CreateDateFilter<T>(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {

        //Check for betweens
        if (filterRequest.FilterType == FilterTypes.BETWEEN || filterRequest.FilterType == FilterTypes.BETWEEN_EXCLUSIVE)
            return CreateBetweenDateFilter(param, filterRequest, getPropertySelectorLambda);


        //Handle everything else
        var originalProperty = PropertyExpressionProvider.GetPropertyExpression(param, filterRequest.Field, getPropertySelectorLambda); //x.Description

        var datePropertyExp = GetSafeDatePropertyExpression(originalProperty);

        if (!DateTime.TryParse(filterRequest.FilterValue, out var outDate))
            return new PgResult<Expression>(false, $"Can't convert {filterRequest.FilterValue} to DateTime value");

        var value = Expression.Constant(outDate.Date);//What to compare with


        var expTypeResult = FilterTypes.GetDateExpressionType(filterRequest.FilterType);
        if (!expTypeResult.Succeeded)
            return expTypeResult.Convert<Expression>();

        var expType = expTypeResult.Value;


        var dateExp = Expression.MakeBinary(expType, datePropertyExp, value);
        var safeDateExp = NullabilityHandler.HandleHasValue<DateTime>(originalProperty, dateExp);

        return new PgResult<Expression>(safeDateExp);

    }


    //-----------------------------------//


    /// <summary>
    /// Creates a filter expression for date ranges using the BETWEEN or BETWEEN_EXCLUSIVE operators.
    /// </summary>
    /// <param name="param">The parameter expression (typically "x => ") to build upon</param>
    /// <param name="filterRequest">The filter request containing the date range values</param>
    /// <param name="getPropertySelectorLambda">Optional function to transform property names</param>
    /// <returns>A result containing either the filter expression or an error message</returns>
    /// <remarks>
    /// The filter value should contain two dates separated by a separator (defined in BetweenHelpers).
    /// Example: "2023-01-01~2023-12-31"
    /// Both inclusive (BETWEEN) and exclusive (BETWEEN_EXCLUSIVE) ranges are supported.
    /// </remarks>
    private static PgResult<Expression> CreateBetweenDateFilter(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {

        var originalProperty = PropertyExpressionProvider.GetPropertyExpression(param, filterRequest.Field, getPropertySelectorLambda); //x.Description
        var datePropertyExp = GetSafeDatePropertyExpression(originalProperty);

        var dateStrings = BetweenHelpers.SplitBetweenValue(filterRequest);

        // we should have 2 dateStrings separated by a betweenSeparator
        if (!BetweenHelpers.IsValidBetweenArray(dateStrings))
            return new PgResult<Expression>(false, $"{filterRequest.FilterValue} is not a valid BETWEEN value");

        var startDateStr = dateStrings[0];
        if (!DateTime.TryParse(startDateStr, out var startDate))
            return new PgResult<Expression>(false, $"Can't convert {startDateStr} of '{filterRequest.FilterValue}' to DateTime value");

        var endDateStr = dateStrings[1];
        if (!DateTime.TryParse(endDateStr, out var endDate))
            return new PgResult<Expression>(false, $"Can't convert {endDateStr} of '{filterRequest.FilterValue}' to DateTime value");


        var betweenExp = BetweenHelpers.BetweenExpression(datePropertyExp, startDate.Date, endDate.Date, filterRequest.FilterType == FilterTypes.BETWEEN);
        var safeBetweenExp = NullabilityHandler.HandleHasValue<DateTime>(originalProperty, betweenExp);

        return new PgResult<Expression>(safeBetweenExp);

    }

    //-----------------------------------//    


    /// <summary>
    /// If <paramref name="originalDatePropertyExp"/> is nullable converts expression to '.Value.Date' type otherwise '.Date' type.
    /// </summary>
    /// <param name="originalDatePropertyExp">DateTime Expression</param>
    /// <returns>Sanitized date value expression</returns>
    /// <remarks>
    /// This method serves two important functions:
    /// 1. Handles nullable DateTime properties by safely accessing their Value
    /// 2. Normalizes all DateTime values to date-only by accessing the Date property (time component set to midnight)
    /// 
    /// For example:
    /// - For non-nullable DateTime properties: x.CreatedDate becomes x.CreatedDate.Date
    /// - For nullable DateTime? properties: x.CreatedDate becomes x.CreatedDate.Value.Date
    /// 
    /// This ensures consistent date comparisons without time components.
    /// </remarks>
    private static MemberExpression GetSafeDatePropertyExpression(MemberExpression originalDatePropertyExp)
    {
        var safeOriginalProperty = NullabilityHandler.HandleNullability<DateTime>(originalDatePropertyExp);

        return Expression.PropertyOrField(safeOriginalProperty, "Date"); //Set it to midnight

    }
}
