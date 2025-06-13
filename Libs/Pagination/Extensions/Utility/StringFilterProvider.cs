using Pagination.Result;
using Pagination.Utility;
using StringHelpers;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Pagination.Extensions.Utility;
internal class StringFilterProvider
{
    internal static PgResult<Expression> CreateStringFilter<T>(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {

        if (filterRequest.FilterType == FilterTypes.IN)
            return CreateStringInFilter(param, filterRequest, getPropertySelectorLambda);


        var propertyExp = PropertyExpressionProvider.GetPropertyExpression(param, filterRequest.Field, getPropertySelectorLambda); //x.Description

        if (propertyExp.Type.IsEnum)
            return HandleStringEnum(propertyExp, filterRequest);

        //Sometimes Id's can be strings on the client and ints on the server
        if (propertyExp.Type != typeof(string))
            return NumericFilterProvider.CreateNumericFilter<T>(param, filterRequest, getPropertySelectorLambda);


        var lowerExp = Expression.Call(propertyExp, StringMethodInfos.ToLower);//x.Description.ToLower()
        var constant = Expression.Constant(filterRequest.FilterValue.ToLower());//What to compare with

        if (filterRequest.FilterType == FilterTypes.EQUALS)
            return new PgResult<Expression>(Expression.MakeBinary(ExpressionType.Equal, lowerExp, constant));

        if (filterRequest.FilterType == FilterTypes.NOT_EQUAL_TO)
            return new PgResult<Expression>(Expression.MakeBinary(ExpressionType.NotEqual, lowerExp, constant));

        var methodInfoResult = StringMethodInfos.GetStringMethodInfo(filterRequest.FilterType);


        if (!methodInfoResult.Succeeded)
            return methodInfoResult.Convert<Expression>();

        var methodInfo = methodInfoResult.Value;


        var stringMethodExp = Expression.Call(
             lowerExp,
             methodInfo!, //Succeeded Value is always non-null
             constant
         ); //x.Description.ToLower().StringMethod(constant)


        //Nulls will cause LINQ to crash so we need to (i.e. null.Contains(filterValue) )
        var nullCheck = Expression.NotEqual(propertyExp, Expression.Constant(null, typeof(string))); //x != null

        var nonNullStringExp = Expression.AndAlso(nullCheck, stringMethodExp); //x.Description != null && x.Description.ToLower().Contains(filterValue)

        return new PgResult<Expression>(nonNullStringExp);
    }

    //-----------------------------------//

    private static PgResult<Expression> CreateStringInFilter(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {
        var property = PropertyExpressionProvider.GetPropertyExpression(param, filterRequest.Field, getPropertySelectorLambda); //x.Description

        ConstantExpression listExp;
        MethodInfo methodInfo;

        if (property.Type != typeof(string))
        {
            //Let this throw an exception. It means the client entered the wrong type.
            //They should find it while testing.
            var convertedList = TryConvertStringList(filterRequest.FilterValues, property.Type);
            listExp = Expression.Constant(convertedList);
            methodInfo = typeof(List<>).MakeGenericType(property.Type).GetMethod("Contains", [property.Type])!;

        }
        else
        {
            listExp = Expression.Constant(filterRequest.FilterValues.ToList());
            methodInfo = typeof(List<string>).GetMethod("Contains", [typeof(string)])!;
        }


        var body = Expression.Call(listExp, methodInfo, property);

        return new PgResult<Expression>(body);

    }

    //-----------------------------------//

    private static IList TryConvertStringList(string[] strList, Type newType)
    {

        Type t = typeof(List<>).MakeGenericType(newType);
        IList convertedList = (IList)Activator.CreateInstance(t)!;


        foreach (var str in strList)
        {
            if (str.IsNullOrWhiteSpace())
                continue;

            if (newType.IsEnum)
                convertedList.Add(Enum.Parse(newType, str));
            else
                convertedList.Add(Convert.ChangeType(str, newType));
        }

        return convertedList;

    }

    //-----------------------------------//

    private static PgResult<Expression> HandleStringEnum(MemberExpression propertyExp, FilterRequest filterRequest)
    {
        var propertyType = propertyExp.Type;


        if (!propertyType.IsEnum)
            return new PgResult<Expression>(false, $"{propertyExp.Type} is not an Enum");


        var expTypeResult = FilterTypes.GetEnumExpressionType(filterRequest.FilterType);

        if (!expTypeResult.Succeeded)
            return expTypeResult.Convert<Expression>();

        var expType = expTypeResult.Value;

        var enumValueExp = Expression.Field(null, propertyType, filterRequest.FilterValue);

        return new PgResult<Expression>(Expression.MakeBinary(expType, propertyExp, enumValueExp));

    }

}
