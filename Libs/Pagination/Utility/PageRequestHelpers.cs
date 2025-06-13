using System.Text;

namespace Pagination.Utility;

public class PageRequestHelpers
{

    public static string PageRequestToString(PagedRequest pr)
         => PageRequestToString(pr.PageNumber, pr.PageSize, pr.SortList, pr.FilterList);

    //-----------------------------//

    public static string PageRequestToString(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest> filterList) => 
        new StringBuilder().
            AppendLine($"{nameof(pageNumber)}: {pageNumber}").
            AppendLine($"{nameof(pageSize)}: {pageSize}").
            AppendLine($"{nameof(sortList)}: {sortList.ListToString()}").
            AppendLine($"{nameof(filterList)}: {filterList.ListToString()}").
            ToString();


}//Cls