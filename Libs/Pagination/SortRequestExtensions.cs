using System.Text;

namespace Pagination;

public static class SortRequestExtensions
{
  public static string ListToString(this IEnumerable<SortRequest> requests)
  {
     if (requests == null || !requests.Any())
        return "";


     var sb = new StringBuilder();
     foreach (var request in requests)
        sb.AppendLine($"{request}");

     return sb.ToString();

  }

}//Cls

