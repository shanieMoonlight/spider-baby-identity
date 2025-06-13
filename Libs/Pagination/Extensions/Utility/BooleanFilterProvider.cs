using Pagination.Result;
using Pagination.Utility;
using System.Linq.Expressions;

namespace Pagination.Extensions.Utility;
internal class BooleanFilterProvider
{
    internal static PgResult<Expression> CreateBooleanFilter<T>(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {
        if (filterRequest.FilterValue == BooleanFilterValues.ALL)
            return new PgResult<Expression>(false, $"No need to filter on Filter Value: {BooleanFilterValues.ALL}");

        var property = PropertyExpressionProvider.GetPropertyExpression(param, filterRequest.Field, getPropertySelectorLambda); //x.Description

        if (!bool.TryParse(filterRequest.FilterValue?.ToLower(), out var outBool))
            return new PgResult<Expression>(false, $"Can't convert {filterRequest.FilterValue} to boolean value");


        var constant = Expression.Constant(outBool);

        return new PgResult<Expression>(Expression.MakeBinary(ExpressionType.Equal, property, constant));

    }

}
