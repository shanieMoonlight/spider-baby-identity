using Pagination.Result;
using Pagination.Utility;
using System.Linq.Expressions;

namespace Pagination.Extensions.Utility;
internal class NumericFilterProvider
{
    internal static PgResult<Expression> CreateNumericFilter<T>(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {
        if (filterRequest.FilterType == FilterTypes.IN)
            return CreateNumericInFilter(param, filterRequest, getPropertySelectorLambda);


        var originalProperty = PropertyExpressionProvider.GetPropertyExpression(param, filterRequest.Field, getPropertySelectorLambda); //x.Description
        var numPropertyExp = NullabilityHandler.HandleNullability<DateTime>(originalProperty); //x.Value.Description (if nullable)


        //Check for betweens
        if (filterRequest.FilterType == FilterTypes.BETWEEN || filterRequest.FilterType == FilterTypes.BETWEEN_EXCLUSIVE)
            return CreateNumericBetweenFilter(filterRequest, numPropertyExp);


        var constantExpResult = ConstantExpressionProvider.CreateNumericConstantExpression(numPropertyExp.Type, filterRequest.FilterValue);//What to compare with
        if (!constantExpResult.Succeeded)
            return PgResult<Expression>.Failure(constantExpResult.Info);

        var value = constantExpResult.Value!; //Succeeded Value is always 

        var expTypeResult = FilterTypes.GetNumericExpressionType(filterRequest.FilterType);


        if (!expTypeResult.Succeeded)
            return expTypeResult.Convert<Expression>();

        var expType = expTypeResult.Value;


        var numberExpression = Expression.MakeBinary(expType, numPropertyExp, value);

        var safeNumberExp = NullabilityHandler.HandleHasValue<DateTime>(originalProperty, numberExpression);

        return new PgResult<Expression>(safeNumberExp);

    }

    //-----------------------------------//

    internal static PgResult<Expression> CreateNumericInFilter(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {
        var property = PropertyExpressionProvider.GetPropertyExpression(param, filterRequest.Field, getPropertySelectorLambda); //x.Description

        var type = NumericEnumerableTypeProvider.GetIEnumerableType(property.Type);
        if (type == null)
            return PgResult<Expression>.Failure("Type not found");

        var methodInfo = type.GetMethod("Contains")!;

        var listExpResult = ConstantExpressionProvider.CreateNumericListConstantExpression(property.Type, filterRequest.FilterValues);
        if (!listExpResult.Succeeded)
            return PgResult<Expression>.Failure(listExpResult.Info);

        var listExp = listExpResult.Value;

        var body = Expression.Call(listExp, methodInfo, property);

        return new PgResult<Expression>(body);

    }

    //-----------------------------------//

    internal static PgResult<Expression> CreateNumericBetweenFilter(FilterRequest filterRequest, MemberExpression property)
    {
        var numStrings = BetweenHelpers.SplitBetweenValue(filterRequest);
        // we should have 2 numStrings separated by a betweenSeparator
        if (!BetweenHelpers.IsValidBetweenArray(numStrings))
            return new PgResult<Expression>(false, $"{filterRequest.FilterValue} is not a valid BETWEEN value");

        var startConstantExpResult = ConstantExpressionProvider.CreateNumericConstantExpression(property.Type, filterRequest.FilterValue);//What to compare with
        if (!startConstantExpResult.Succeeded)
            return PgResult<Expression>.Failure(startConstantExpResult.Info);

        var startConstantExp = startConstantExpResult.Value!;//Succeeded Value is always non-null

        var endConstantExpResult = ConstantExpressionProvider.CreateNumericConstantExpression(property.Type, filterRequest.FilterValue);//What to compare with
        if (!endConstantExpResult.Succeeded)
            return PgResult<Expression>.Failure(endConstantExpResult.Info);

        var endConstantExp = endConstantExpResult.Value!;//Succeeded Value is always non-null


        var between = BetweenHelpers.BetweenExpression(property, startConstantExp, endConstantExp, filterRequest.FilterType == FilterTypes.BETWEEN);

        return new(between);
    }


}
