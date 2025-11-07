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

        //Sometimes Id's can be strings on the client and ints/guids on the server
        if (propertyExp.Type == typeof(Guid))
            return GuidFilterProvider.CreateGuidFilter<T>(param, filterRequest, getPropertySelectorLambda);

        if (propertyExp.Type != typeof(string))
            return NumericFilterProvider.CreateNumericFilter<T>(param, filterRequest, getPropertySelectorLambda);


        // Coalesce null property to empty string before ToLower to avoid NullReferenceException
        var safePropertyExp = Expression.Coalesce(propertyExp, Expression.Constant(string.Empty, typeof(string))); // x.Description ?? ""
        var lowerExp = Expression.Call(safePropertyExp, StringMethodInfos.ToLower); // (x.Description ?? "").ToLower()
        var constant = Expression.Constant(filterRequest.FilterValue.Trim().ToLower());//What to compare with

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
         ); // (x.Description ?? "").ToLower().StringMethod(constant)


        // With coalescing above we no longer need an explicit null-check; null is treated as empty string
        return new PgResult<Expression>(stringMethodExp);
    }

    //-----------------------------------//

    private static PgResult<Expression> CreateStringInFilter(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {
        var property = PropertyExpressionProvider.GetPropertyExpression(param, filterRequest.Field, getPropertySelectorLambda); //x.Description

        ConstantExpression listExp;
        MethodInfo methodInfo;

        //Sometimes Id's can be strings on the client and ints/guids on the server
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

        // Support nullable types by using underlying type when present
        var targetType = Nullable.GetUnderlyingType(newType) ?? newType;

        Type t = typeof(List<>).MakeGenericType(targetType);
        IList convertedList = (IList)Activator.CreateInstance(t)!;


        foreach (var str in strList)
        {
            if (string.IsNullOrWhiteSpace(str))
                continue;

            if (targetType.IsEnum)
                convertedList.Add(Enum.Parse(targetType, str));
            else if (targetType == typeof(Guid))
                convertedList.Add(Guid.Parse(str));
            else
                convertedList.Add(Convert.ChangeType(str, targetType));
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

}//Cls