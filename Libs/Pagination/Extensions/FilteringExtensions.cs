using CollectionHelpers;
using Microsoft.Extensions.Logging;
using Pagination.Extensions.Utility;
using Pagination.Result;
using Pagination.Utility;
using StringHelpers;
using System.Linq.Expressions;

namespace Pagination.Extensions;

public static class FilteringExtensions
{
    /// <summary>
    /// Sorts <paramref name="list"/> according to the details in <paramref name="filterRequestList"/>
    /// </summary>
    /// <typeparam name="T">The type of objects being sorted</typeparam>
    /// <param name="list">The items to filter</param>
    /// <param name="filterRequestList">List of fields to filter by</param>
    /// <returns>The same list with filtering appended to it</returns>
    public static IEnumerable<T> AddFiltering<T>(this IEnumerable<T> list, IEnumerable<FilterRequest> filterRequestList, ILogger? logger = null) =>
        list.AddFiltering(filterRequestList, null, logger);

    //-----------------------------------//

    /// <summary>
    /// Sorts <paramref name="list"/> according to the details in <paramref name="filterRequestList"/>
    /// </summary>
    /// <typeparam name="T">The type of objects being sorted</typeparam>
    /// <param name="list">The items to filter</param>
    /// <param name="filterRequestList">List of fields to filter by</param>
    /// <returns>The same list with filtering appended to it</returns>
    public static IEnumerable<T> AddFiltering<T>(this IEnumerable<T> list, IEnumerable<FilterRequest>? filterRequestList, Func<string, string>? getPropertySelectorLambda, ILogger? logger = null)
    {
        //Don't waste time.
        if (list.IsFilteringUnnecessary(filterRequestList))
            return list;

        return list.AsQueryable()
            .AddFiltering(filterRequestList, getPropertySelectorLambda, logger)
            .AsEnumerable();
    }

    //-----------------------------------//

    /// <summary>
    /// Sorts <paramref name="list"/> according to the details in <paramref name="filterRequestList"/>
    /// </summary>
    /// <typeparam name="T">The type of objects being sorted</typeparam>
    /// <param name="list">The items to filter</param>
    /// <param name="filterRequestList">List of fields to filter by</param>
    /// <returns>The same list with filtering appended to it</returns>
    public static IQueryable<T> AddFiltering<T>(this IQueryable<T> list, IEnumerable<FilterRequest>? filterRequestList, ILogger? logger = null) =>
        list.AddFiltering(filterRequestList, null, logger);

    //-----------------------------------//

    /// <summary>
    /// Sorts <paramref name="list"/> according to the details in <paramref name="filterRequestList"/>
    /// </summary>
    /// <typeparam name="T">The type of objects being sorted</typeparam>
    /// <param name="list">The items to filter</param>
    /// <param name="filterRequestList">List of fields to filter by</param>
    /// <returns>The same list with filtering appended to it</returns>
    public static IQueryable<T> AddFiltering<T>(
        this IQueryable<T> list, IEnumerable<FilterRequest>? filterRequestList, Func<string, string>? getPropertySelectorLambda, ILogger? logger = null)
    {

        //Don't waste time.
        if (list.IsFilteringUnnecessary(filterRequestList))
            return list;


        // The IQueryable data to query.  
        IQueryable<T> queryableData = list;

        var param = Expression.Parameter(typeof(T), "x"); //x =>

        Expression andExpression = Expression.Constant(true);

        foreach (var fr in filterRequestList!)
        {
            //Just skip if it's not filled in properly
            if (IsFilterRequestInvalid(fr))
                continue;

            var filterResult = CreateFilter<T>(param, fr, getPropertySelectorLambda);
            if (!filterResult.Succeeded)
                //Log it and skip this level
                logger?.LogError(new EventId(60606, "Unknown Dev Error"), "{msg}", filterResult?.ToString());
            else
                andExpression = Expression.AndAlso(andExpression, filterResult.Value!);
        }


        var whereExpression = Expression.Call(
                       typeof(Queryable),
                       "Where",
                       [queryableData.ElementType],
                       queryableData.Expression,
                       Expression.Lambda<Func<T, bool>>(andExpression, [param])
                    );


        return queryableData.Provider.CreateQuery<T>(whereExpression);
    }

    //-----------------------------------//

    /// <summary>
    /// Checks whether this list needs to be filtered <para />
    /// It checks whether running this list through all the filtering in <paramref name="filterRequest"/> would actually change anything
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="filterRequest"></param>
    /// <returns>Whether filtering this list would change anything</returns>
    public static bool IsFilteringUnnecessary<T>(this IEnumerable<T>? list, IEnumerable<FilterRequest>? filterRequest)
    {
        if (list == null) 
            return true;

        if (!filterRequest.AnyValues()) 
            return true;

        if (!filterRequest!.Any(fr => fr.AnyValues()))
            return true;

        return false;
    }

    //-----------------------------------//

    private static bool IsFilterRequestInvalid(FilterRequest fr)
    {
        if (fr == null)
            return false;
        if (string.IsNullOrWhiteSpace(fr.FilterType))
            return true;
        if (fr.FilterType == FilterTypes.NONE)
            return true;
        if (string.IsNullOrWhiteSpace(fr.FilterValue) && !fr.FilterValues.AnyValues())
            return true;
        if (fr.FilterType == FilterTypes.IN && !fr.FilterValues.AnyValues())
            return true;
        //Between types must have a between separator in the value
        if (fr.FilterType == FilterTypes.BETWEEN && !BetweenHelpers.IsValidBetweenValue(fr.FilterValue))
            return true;
        //if ((fr.FilterType != FilterTypes.BETWEEN && BetweenHelpers.IsValidBetweenValue(fr.FilterValue)))
        //    return true;
        if (fr.FilterDataType == FilterDataTypes.BOOLEAN && !FilterTypes.BOOLEAN_TYPES.Contains(fr.FilterType))
            return true;
        if (fr.FilterDataType == FilterDataTypes.DATE && !FilterTypes.DATE_TYPES.Contains(fr.FilterType))
            return true;
        if (fr.FilterDataType == FilterDataTypes.NUMBER && !FilterTypes.NUMBER_TYPES.Contains(fr.FilterType))
            return true;
        if (fr.FilterDataType == FilterDataTypes.STRING && !FilterTypes.STRING_TYPES.Contains(fr.FilterType))
            return true;
        if (fr.FilterValue == BooleanFilterValues.ALL)
            return true;
        if (fr.FilterValues.IsNullOrWhiteSpace() && fr.FilterType == FilterTypes.IN)
            return true;

        return false;
    }

    //-----------------------------------//

    /// <summary>
    /// Create a level of filtering for the linq query (Where)
    /// </summary>
    /// <typeparam name="T">Type of objects being filtered</typeparam>
    /// <param name="param">The paramater part of the expression that will be built</param>
    /// <param name="filterRequest">filtering details</param>
    /// <param name="getPropertySelectorLambda">Converts the field to the appropriate name for the property</param>
    /// <returns>The same list with filtering appended to it</returns>
    private static PgResult<Expression> CreateFilter<T>(ParameterExpression param, FilterRequest filterRequest, Func<string, string>? getPropertySelectorLambda = null)
    {
        return filterRequest.FilterDataType.ToLower() switch
        {
            FilterDataTypes.BOOLEAN => BooleanFilterProvider.CreateBooleanFilter<T>(param, filterRequest, getPropertySelectorLambda),
            FilterDataTypes.DATE => DateFilterProvider.CreateDateFilter<T>(param, filterRequest, getPropertySelectorLambda),
            FilterDataTypes.NUMBER => NumericFilterProvider.CreateNumericFilter<T>(param, filterRequest, getPropertySelectorLambda),
            FilterDataTypes.STRING => StringFilterProvider.CreateStringFilter<T>(param, filterRequest, getPropertySelectorLambda),
            _ => NumericFilterProvider.CreateNumericFilter<T>(param, filterRequest, getPropertySelectorLambda),
        };
    }

}//Cls