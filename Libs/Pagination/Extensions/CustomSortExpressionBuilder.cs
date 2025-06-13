using CollectionHelpers;
using System.Linq.Expressions;

namespace Pagination.Extensions;

/// <summary>
/// Build Custom sort expression to handle Inner properties of differences betwwen client naming and Server naming 
/// 
/// Example:
/// var data = _db
///         .Set<T>()
///         .AddFiltering(filterList, GetFilteringPropertySelectorLambda)
///         .AddEfSorting(sortList, GetCustomSortBuider())
///         .AsNoTracking();   //, GetSortingPropertySelectorLambda);
/// 
/// protected override CustomSortExpressionBuilder<Product> GetCustomSortBuider() =>
///     CustomSortExpressionBuilder<Product>
///         .Create(nameof(Product.Category), (prd) => prd.Product.Category.Name)
///         .AddCustomSort("Name", prd => prd.Product.DisplayName");
/// 
/// 
/// </summary>
public class CustomSortExpressionBuilder<T>
{
    private readonly InvariantStringDictionary< Expression<Func<T, object>>> _expressionMap = [];

    //-----------------------------------//

    private CustomSortExpressionBuilder() { }

    //- - - - - - - - - - - - - - - - - - //

    public static CustomSortExpressionBuilder<T> Create() => new();

    //- - - - - - - - - - - - - - - - - - //

    public static CustomSortExpressionBuilder<T> Create(string field, Expression<Func<T, object>> sortValueFunc) => 
        new CustomSortExpressionBuilder<T>()
            .AddCustomSort(field, sortValueFunc);

    //-----------------------------------//

    public CustomSortExpressionBuilder<T> AddCustomSort(string field, Expression<Func<T, object>> sortValueFunc)
    {
        //overwrite anything that was already there.
        _expressionMap[field.ToLower()] = sortValueFunc;
        return this;
    }

    //-----------------------------------//

    public Expression<Func<T, object>> GetSorter(string field) => 
        _expressionMap.TryGetValue(field, out var value)
           ? value ?? field.ToDefaultPropertySortExpression<T>() //Just in case someone entered a null Func
           : field.ToDefaultPropertySortExpression<T>();

}//Cls

