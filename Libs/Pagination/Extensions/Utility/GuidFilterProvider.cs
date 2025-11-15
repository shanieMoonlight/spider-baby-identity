using Pagination.Result;
using System.Linq.Expressions;


namespace Pagination.Extensions.Utility;


internal class GuidFilterProvider
{
    internal static PgResult<Expression> CreateGuidFilter<T>(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {
        if (filterRequest.FilterType == FilterTypes.IN)
            return CreateGuidInFilter(param, filterRequest, getPropertySelectorLambda);

        var propertyExp = PropertyExpressionProvider.GetPropertyExpression(param, filterRequest.Field, getPropertySelectorLambda);

        var guidValue = Guid.Parse(filterRequest.FilterValue);
        var constant = Expression.Constant(guidValue);

        ExpressionType expType = filterRequest.FilterType switch
        {
            FilterTypes.EQUALS => ExpressionType.Equal,
            FilterTypes.NOT_EQUAL_TO => ExpressionType.NotEqual,
            _ => throw new NotSupportedException($"FilterType {filterRequest.FilterType} not supported for GUIDs")
        };

        var comparisonExp = Expression.MakeBinary(expType, propertyExp, constant);
        return new PgResult<Expression>(comparisonExp);
    }

    //------------------------//

    internal static PgResult<Expression> CreateGuidInFilter(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {
        // Convert all filter values to Guid
        var guidList = filterRequest.FilterValues
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(Guid.Parse)
            .ToList();

        var property = PropertyExpressionProvider.GetPropertyExpression(param, filterRequest.Field, getPropertySelectorLambda);
        var listExp = Expression.Constant(guidList);
        var methodInfo = typeof(List<Guid>).GetMethod("Contains", [typeof(Guid)])!;
        var body = Expression.Call(listExp, methodInfo, property);
        return new PgResult<Expression>(body); ;

    }

}
