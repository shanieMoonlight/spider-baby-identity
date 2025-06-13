using CollectionHelpers;

namespace Pagination.Extensions;

/// <summary>
/// Build Custom sort Function to handle Inner properties of differences betwwen client naming and Server naming 
/// 
/// Example:
/// var data  =someEnumerable
///         .AddFiltering(filterList, GetFilteringPropertySelectorLambda)
///         .AddEfSorting(sortList, GetCustomSortBuider())
///         .AsNoTracking();   //, GetSortingPropertySelectorLambda);
/// 
/// protected override CustomSortFuncBuilder<Product> GetCustomSortBuider() =>
///     CustomSortFuncBuilder<Product>
///         .Create(nameof(Product.Category), prd => prd.Product.Category.Name)
///         .AddCustomSort("Name", prd => prd.Product.DisplayName");
/// 
/// 
/// </summary>
public class CustomSortFuncBuilder<T>
{
    private readonly InvariantStringDictionary<Func<T, object>> _funcMap = [];

    //-----------------------------//

    private CustomSortFuncBuilder() { }

    //- - - - - - - - - - - - - - - - - //

    public static CustomSortFuncBuilder<T> Create() => new();

    //- - - - - - - - - - - - - - - - - //

    public static CustomSortFuncBuilder<T> Create(string field, Func<T, object> sortValueFunc) =>
        new CustomSortFuncBuilder<T>()
            .AddCustomSort(field, sortValueFunc);

    //-----------------------------//

    public CustomSortFuncBuilder<T> AddCustomSort(string field, Func<T, object> sortValueFunc)
    {
        //overwrite anything that was already there.
        _funcMap[field.ToLower()] = sortValueFunc;
        return this;
    }

    //-----------------------------//

    public Func<T, object> GetSortFunc(string field) =>
        _funcMap.TryGetValue(field, out var value)
           ? value ?? field.ToDefaultPropertySortFunc<T>() //Just in case someone entered a null Func
           : field.ToDefaultPropertySortFunc<T>();

}//Cls
