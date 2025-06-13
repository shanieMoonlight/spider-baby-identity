using Pagination.Result;
using System.Reflection;

namespace Pagination.Utility;

/// <summary>
/// GetMethod uses reflection (slow).
/// This class contains static GetMethods so that reflection is only used once.
/// </summary>
public class StringMethodInfos
{
    public static readonly MethodInfo Contains = typeof(string).GetMethod("Contains", [typeof(string)])!;
    public static readonly MethodInfo StartsWith = typeof(string).GetMethod("StartsWith", [typeof(string)])!;
    public static readonly MethodInfo EndsWith = typeof(string).GetMethod("EndsWith", [typeof(string)])!;
    public static readonly MethodInfo ToLower = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
    public static readonly MethodInfo ToUpper = typeof(string).GetMethod("ToUpper", Type.EmptyTypes)!;

    private static readonly Dictionary<string, MethodInfo> _filterTypeToMethodInfoMap =
        new(){
            { FilterTypes.CONTAINS, Contains },
            { FilterTypes.STARTS_WITH, StartsWith },
            { FilterTypes.ENDS_WITH, EndsWith }
           };


    //-----------------------------//

    internal static PgResult<MethodInfo> GetStringMethodInfo(string filterType)
    {

        if (_filterTypeToMethodInfoMap.TryGetValue(filterType, out MethodInfo? value))
            return new PgResult<MethodInfo>(value);

        return new PgResult<MethodInfo>(false, $"FilterType, {filterType} not found");

    }

    //-----------------------------//

}//Cls
