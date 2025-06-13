using CollectionHelpers;
using StringHelpers;
using System.Text;

namespace Pagination;

public class FilterRequest
{

    /// <summary>
    /// What field/column to filter by
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// What type of filter to use (EQUALS, START_WITH, etc.)
    /// Defaults to EQUALS
    /// </summary>
    public string FilterType { get; set; } = FilterTypes.EQUALS;

    /// <summary>
    /// What to compare/filter with
    /// </summary>
    public string FilterValue { get; set; } = string.Empty;


    /// <summary>
    /// What to compare/filter with
    /// </summary>
    public string[] FilterValues { get; set; } = [];

    /// <summary>
    /// What type of data are we filtering
    /// </summary>
    public string FilterDataType { get; set; } = FilterDataTypes.STRING;

    //-----------------------------//

    public override string ToString()
    {
        var fvString = "";
        if (FilterValues.AnyValues())
            fvString = Environment.NewLine + string.Join($",{Environment.NewLine}", FilterValues);


        return
            new StringBuilder()
       .AppendLine($"{nameof(Field)}:{Field}")
       .AppendLine($"{nameof(FilterValue)}:{FilterValue}")
       .AppendLine($"{nameof(FilterValues)}:{FilterValues?.Length}{fvString}")
       .AppendLine($"{nameof(FilterDataType)}:{FilterDataType}")
       .ToString();

    }

    //-----------------------------//

    /// <summary>
    /// Is/Are there any FilterValue/s 
    /// </summary>
    public bool AnyValues() =>
        !FilterValue.IsNullOrWhiteSpace()
        ||
        FilterType == FilterTypes.IN && FilterValues.AnyValues();


}//Cls

