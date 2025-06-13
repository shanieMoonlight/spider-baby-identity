using ExpressionHelpers;
using CollectionHelpers;
using StringHelpers;
using System.Linq.Expressions;

namespace Pagination.Extensions;

internal static class SortingHelpers
{
    /// <summary>
    /// Check if it's worth sorting
    /// </summary>
    internal static bool IsSortRequestInvalid<T>(IEnumerable<T> list, IEnumerable<SortRequest> sortRequestList) =>
        !sortRequestList.AnyValues()
            || !list.AnyValues()
            || !sortRequestList.ToSafeSortRequestList().Any();

    //-----------------------------//

    /// <summary>
    /// Removes any nonsense queries and returns the good stuff.
    /// </summary>
    internal static IEnumerable<SortRequest> ToSafeSortRequestList(this IEnumerable<SortRequest> sortRequestList)
        => sortRequestList?.Where(s => !s.Field.IsNullOrWhiteSpace()) ?? [];

    //-----------------------------//

    public static Expression<Func<T, object>> ToDefaultPropertySortExpression<T>(this string field)
    {

        var param = Expression.Parameter(typeof(T), "x"); //x =>
        var propertyName = field.CamelToPascal();

        var memberExp = param.ToMemberExpression(propertyName);
        var memberExpObj = Expression.Convert(memberExp, typeof(object)); //Prevents exception on Nullable Types

        return Expression.Lambda<Func<T, object>>(memberExpObj, param);

    }

    //-----------------------------//

    internal static Func<T, object?> ToDefaultPropertySortFunc<T>(this string field) =>
        field.ToDefaultPropertySortExpression<T>()
        .Compile();


}//Cls
