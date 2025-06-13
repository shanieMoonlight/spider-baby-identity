using System.Linq.Expressions;

namespace Pagination.Extensions;

public static class SortingQueryableExtensions
{


    /// <summary>
    /// Sorts <paramref name="list"/> according to the details in <paramref name="sortRequestList"/>
    /// </summary>
    /// <typeparam name="T">The type of objects being sorted</typeparam>
    /// <param name="list">The items to sort</param>
    /// <param name="sortRequestList">List of fields to sort by</param>
    /// <param name="sortExpBuider">Custom sorting. Any fields that should be mapped to inner properties etc. Will default to [T => T.Field] if builder is null or the builder
    /// returns null for the specific field
    /// </param>
    /// <returns>The same list with sorting appended to it</returns>
    public static IQueryable<T> AddEfSorting<T>(this IQueryable<T> list, IEnumerable<SortRequest> sortRequestList, CustomSortExpressionBuilder<T>? sortExpBuider = null)
    {
        if (SortingHelpers.IsSortRequestInvalid(list, sortRequestList))
            return list;

        var isFirst = true;
        sortExpBuider ??= CustomSortExpressionBuilder<T>.Create();

        foreach (var sr in sortRequestList.ToSafeSortRequestList())
        {
            Expression<Func<T, object>> lmda  = sortExpBuider.GetSorter(sr.Field);
            list = list.AddSortLevel(sr.SortDescending, lmda, isFirst);
            isFirst = false;
        }

        return list;

    }

    //-----------------------------//

    /// <summary>
    /// Adds a level of sorting to the linq queue
    /// </summary>
    /// <typeparam name="T">Type of objects being sorted</typeparam>
    /// <param name="list">The Items being sorted</param>
    /// <param name="sortDescending">What direction to sort</param>
    /// <param name="propertySelectorLambda">Function dexcribing what property to sort on.</param>
    /// <param name="isFirst">Is this the first item in the LINQ sort queue</param>
    /// <returns>The same list with sorting appended to it</returns>
    private static IOrderedQueryable<T> AddSortLevel<T>(this IQueryable<T> list, bool sortDescending, Expression<Func<T, object>> propertySelectorExp, bool isFirst) =>
        isFirst
            ? list.FirstSortLevel(sortDescending, propertySelectorExp)
            : (list as IOrderedQueryable<T>).NextSortLevel(sortDescending, propertySelectorExp);

    //-----------------------------//

    /// <summary>
    /// Adds a level of sorting to the linq queue (OrderBy)
    /// </summary>
    /// <typeparam name="T">Type of objects being sorted</typeparam>
    /// <param name="list">The Items being sorted</param>
    /// <param name="sortDescending">What direction to sort</param>
    /// <param name="propertySelectorLambdaExp">Function dexcribing what property to sort on.</param>
    /// <returns>The same list with sorting appended to it</returns>
    private static IOrderedQueryable<T> FirstSortLevel<T>(this IQueryable<T> list, bool sortDescending, Expression<Func<T, object>> propertySelectorExp) =>
        sortDescending
            ? list.OrderByDescending(propertySelectorExp)
            : list.OrderBy(propertySelectorExp);

    //-----------------------------//

    /// <summary>
    /// Adds a level of sorting to the linq queue (ThenBy)
    /// </summary>
    /// <typeparam name="T">Type of objects being sorted</typeparam>
    /// <param name="list">The Items being sorted</param>
    /// <param name="sortDescending">What direction to sort</param>
    /// <param name="propertySelectorLambdaExp">Function dexcribing what property to sort on.</param>
    /// <returns>The same list with sorting appended to it</returns>
    private static IOrderedQueryable<T> NextSortLevel<T>(this IOrderedQueryable<T> list, bool sortDescending, Expression<Func<T, object>> propertySelectorExp) =>
        sortDescending
        ? list.ThenByDescending(propertySelectorExp)
        : list.ThenBy(propertySelectorExp);

    //-----------------------------//

}//Cls