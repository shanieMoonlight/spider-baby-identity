using System.Text;

namespace Pagination;

public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = [];

    public int? PageNumber { get; set; }

    public int? PageSize { get; set; }

    public int? TotalPages { get; set; }

    public int? TotalItems { get; set; }

    public PagedRequest? NextPageRequest { get; set; } = null;

    public PagedRequest? PreviousPageRequest { get; set; } = null;

    //-----------------------------//

    private PagedResponse() { }

    //- - - - - - - - - - - - - - -//

    public PagedResponse(Page<T> page, PagedRequest request)
    {
        Data = page.Data;
        PageNumber = page.Number;
        PageSize = page.Size;
        TotalPages = page.TotalPages;
        TotalItems = page.TotalItems;

        if (page.HasNext)
            NextPageRequest = request?.GenerateNextRequest();
        if (page.HasPrevious)
            PreviousPageRequest = request?.GeneratePreviousRequest();
    }

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
    public PagedResponse<U> Transform<U>(Func<T, U> selector) => 
        new()
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            TotalItems = TotalItems,
            TotalPages = TotalPages,
            Data = Data.Select(selector)
        };

    //-----------------------------//

    public override string ToString() => 
        new StringBuilder()
           .AppendLine($"{nameof(PageNumber)}:{PageNumber}")
           .AppendLine($"{nameof(PageSize)}:{PageSize}")
           .AppendLine($"{nameof(TotalItems)}:{TotalItems}")
           .AppendLine($"{nameof(TotalPages)}:{TotalPages}")
           .AppendLine($"{nameof(Data)} Count:{Data?.Count()}")
           .ToString();


}//Cls