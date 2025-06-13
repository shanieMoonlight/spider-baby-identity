namespace Pagination.Extensions;

public static class SortingEnumerableExtensions
{
    //-----------------------------//

    /// <summary>
    /// Sorts <paramref name="list"/> according to the details in <paramref name="sortRequestList"/>
    /// </summary>
    /// <typeparam name="T">The type of objects being sorted</typeparam>
    /// <param name="list">The items to sort</param>
    /// <param name="sortRequestList">List of fields to sort by</param>
    /// <param name="sortFuncBuider">Custom sorting. Any fields that should be mapped to inner properties etc. Will default to [T => T.Field] if builder is null or the builder
    /// returns null for the specific field
    /// </param>
    /// <returns>The same list with sorting appended to it</returns>
    public static IEnumerable<T> AddSorting<T>(this IEnumerable<T> list, IEnumerable<SortRequest> sortRequestList, CustomSortFuncBuilder<T>? sortFuncBuider = null)
    {
        if (SortingHelpers.IsSortRequestInvalid(list, sortRequestList))
            return list;

        var isFirst = true;
        sortFuncBuider ??= CustomSortFuncBuilder<T>.Create();

        foreach (var sr in sortRequestList.ToSafeSortRequestList())
        {
            Func<T, object> lmda = sortFuncBuider.GetSortFunc(sr.Field);
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
    private static IOrderedEnumerable<T> AddSortLevel<T>(this IEnumerable<T> list, bool sortDescending, Func<T, object> propertySelectorLambda, bool isFirst) => 
        isFirst
            ? list.FirstSortLevel(sortDescending, propertySelectorLambda)
            : (list as IOrderedEnumerable<T>).NextSortLevel(sortDescending, propertySelectorLambda);

    //-----------------------------//

    /// <summary>
    /// Adds a level of sorting to the linq queue (OrderBy)
    /// </summary>
    /// <typeparam name="T">Type of objects being sorted</typeparam>
    /// <param name="list">The Items being sorted</param>
    /// <param name="sortDescending">What direction to sort</param>
    /// <param name="propertySelectorLambda">Function dexcribing what property to sort on.</param>
    /// <returns>The same list with sorting appended to it</returns>
    private static IOrderedEnumerable<T> FirstSortLevel<T>(this IEnumerable<T> list, bool sortDescending, Func<T, object> propertySelectorLambda) => 
        sortDescending
            ? list.OrderByDescending(propertySelectorLambda)
            : list.OrderBy(propertySelectorLambda);

    //-----------------------------//

    /// <summary>
    /// Adds a level of sorting to the linq queue (ThenBy)
    /// </summary>
    /// <typeparam name="T">Type of objects being sorted</typeparam>
    /// <param name="list">The Items being sorted</param>
    /// <param name="sortDescending">What direction to sort</param>
    /// <param name="propertySelectorLambda">Function dexcribing what property to sort on.</param>
    /// <returns>The same list with sorting appended to it</returns>
    private static IOrderedEnumerable<T> NextSortLevel<T>(this IOrderedEnumerable<T> list, bool sortDescending, Func<T, object> propertySelectorLambda) => 
        sortDescending
            ? list.ThenByDescending(propertySelectorLambda)
            : list.ThenBy(propertySelectorLambda);

    //-----------------------------//

}//Cls