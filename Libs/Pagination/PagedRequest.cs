using CollectionHelpers;
using System.Text;

namespace Pagination;

public class PagedRequest
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public FilterRequest[] FilterList { get; set; } = [];
    public SortRequest[] SortList { get; set; } = [];

    //-----------------------------//

    public PagedRequest()
    {
        PageNumber = DefaultValues.PAGE_NUMBER;
        PageSize = DefaultValues.PAGE_SIZE;
        FilterList = [];
        SortList = [];
    }

    //- - - - - - - - - - - - - - - - -//

    public PagedRequest(int pageNumber, int pageSize)
    {
        //Negative numbers make no sense
        PageNumber = Math.Max(pageNumber, 0);
        PageSize = pageSize;
    }

    //-----------------------------//

    internal PagedRequest GenerateNextRequest()
    {
        return new PagedRequest(PageNumber + 1, PageSize)
        {
            FilterList = FilterList,
            SortList = SortList
        };

    }

    //-----------------------------//

    internal PagedRequest GeneratePreviousRequest()
    {
        return new PagedRequest(PageNumber - 1, PageSize)
        {
            FilterList = FilterList,
            SortList = SortList
        };

    }

    //-----------------------------//


    public static PagedRequest Empty() => new();


    //-----------------------------//

    public override string ToString()
    {
        var filterListStr = "";
        if (FilterList.AnyValues())
            filterListStr = Environment.NewLine + string.Join<FilterRequest>($",{Environment.NewLine}", FilterList);

        var sortListStr = "";
        if (SortList.AnyValues())
            sortListStr = Environment.NewLine + string.Join<SortRequest>($",{Environment.NewLine}", SortList);


        return
            new StringBuilder()
       .AppendLine($"{nameof(PageNumber)}:{PageNumber}")
       .AppendLine($"{nameof(PageSize)}:{PageSize}")
       .AppendLine($"{nameof(FilterList)}:{filterListStr}")
       .AppendLine($"{nameof(SortList)}:{sortListStr}")
       .ToString();

    }

}//Cls
