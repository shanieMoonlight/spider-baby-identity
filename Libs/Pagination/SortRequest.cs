namespace Pagination;

public class SortRequest
{
  /// <summary>
  /// What field/column to sort by
  /// </summary>
  public string Field { get; set; } = string.Empty;

  /// <summary>
  /// Whether to sort by descending or not.
  /// Defaults to false
  /// </summary>
  public bool SortDescending { get; set; } = false;


    //-----------------------------//

    public override string ToString() => @$"
Field: {Field}
SortDescending: {SortDescending}
";


}//Cls

