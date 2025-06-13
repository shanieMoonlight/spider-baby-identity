using System.Text;

namespace Pagination;


public static class FilterRequestExtensions
{
  public static string ListToString(this IEnumerable<FilterRequest>? requests)
  {
     if (requests == null || !requests.Any())
        return string.Empty;


     var sb = new StringBuilder();
     foreach (var request in requests)
        sb.AppendLine($"{request}");

     return sb.ToString();

  }

}//Cls


