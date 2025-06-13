namespace Pagination;

public class Page<T>
{
    /// <summary>
    /// The payload
    /// </summary>
    public IList<T> Data { get; set; } = [];

    /// <summary>
    /// How many rows in page
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Which page is this
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// How many pages are available in total
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// How items are available in total
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// IS there a Next Page
    /// </summary>
    public bool HasNext { get { return Number < TotalPages; } }

    /// <summary>
    /// IS there a Previous Page
    /// </summary>
    public bool HasPrevious { get { return Number > 1; } }

    //-----------------------------//

    /// <summary>
    /// Creates new page as a section of <paramref name="allData"/>
    /// This ctor will enable better SQL Queries with EfCore
    /// </summary>
    /// <param name="allData">How much data is there in total (How big is the book)</param>
    /// <param name="pageNumber">What page is this</param>
    /// <param name="pageSize">How many rows/lines in the page</param>
    public Page(IQueryable<T> allData, int pageNumber, int pageSize) : this(allData.Count(), pageNumber, pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;

        Data = allData
        ?.Skip(skip)
        ?.Take(pageSize)
        ?.ToList() ?? [];
    }

    //- - - - - - - - - - - - - - -//

    /// <summary>
    /// Creates new page as a section of <paramref name="allData"/>
    /// </summary>
    /// <param name="allData">How much data is there in total (How big is the book)</param>
    /// <param name="pageNumber">What page is this</param>
    /// <param name="pageSize">How many rows/lines in the page</param>
    public Page(IEnumerable<T> allData, int pageNumber, int pageSize) : this(allData.Count(), pageNumber, pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;

        Data = allData
        ?.Skip(skip)
        ?.Take(pageSize)
        ?.ToList() ?? [];
    }

    //- - - - - - - - - - - - - - -//

    /// <summary>
    /// Creates new page as a section of <paramref name="totalItems"/>
    /// </summary>
    /// <param name="totalItems">How much data is there in total (How big is the book)</param>
    /// <param name="pageNumber">What page is this</param>
    /// <param name="pageSize">How many rows/lines in the page</param>
    public Page(int totalItems, int pageNumber, int pageSize)
    {
        Number = pageNumber;
        Size = pageSize;

        TotalItems = totalItems;

        double pagesExact = TotalItems / (double)pageSize;
        TotalPages = (int)Math.Ceiling(pagesExact);
    }

    //- - - - - - - - - - - - - - -//

    public Page() { }

    //-----------------------------//

    /// <summary>
    /// Transforms the page data into objects of type <typeparamref name="U"/> and returns the data.<para />
    /// Usually Model-> DTO
    /// </summary>
    /// <typeparam name="U">New data type</typeparam>
    /// <param name="selector">Function for transforming type <typeparamref name="T"/> into <typeparamref name="U"/></param>
    /// <returns>Transformed Data</returns>
    private List<U> TransformData<U>(Func<T, U> selector) =>
        [.. Data.Select(selector)];

    //-----------------------------//

    /// <summary>
    /// Transforms the page data into objects of type <typeparamref name="U"/> and returns the page.<para />
    /// Usually Model-> DTO
    /// Use this after the Model Page has been created so that the Select function will only be run on 
    /// Size elements rather than all the elements 
    /// </summary>
    /// <typeparam name="U">New data type</typeparam>
    /// <param name="selector">Function for transforming type <typeparamref name="T"/> into <typeparamref name="U"/></param>
    /// <returns>The same page with the data transformed</returns>
    public Page<U> Transform<U>(Func<T, U> selector)
    {
        return new()
        {
            Number = Number,
            Size = Size,
            TotalItems = TotalItems,
            TotalPages = TotalPages,
            Data = TransformData(selector)
        };
    }

}//Cls