using Pagination.Result;
using System.Linq.Expressions;

namespace Pagination;

/// <summary>
/// Different ways to filter data
/// </summary>
public class FilterTypes
{

    public const string EQUALS = "equals";
    public const string STARTS_WITH = "starts_with";
    public const string ENDS_WITH = "ends_with";
    public const string CONTAINS = "contains";
    public const string GREATER_THAN = "greater_than";
    public const string LESS_THAN = "less_than";

    public const string LESS_THAN_OR_EQUAL_TO = "less_than_or_equal_to";
    public const string GREATER_THAN_OR_EQUAL_TO = "greater_than_or_equal_to";
    public const string NOT_EQUAL_TO = "not_equal_to";
    public const string BETWEEN = "between";
    public const string BETWEEN_EXCLUSIVE = "between_exclusive";
    //public const string IS_NULL = "is_null";
    //public const string IS_EMPTY = "is_empty";
    public const string IN = "in";
    //public const string IS_NOT_NULL = "is_not_null";
    //public const string IS_NOT_EMPTY = "is_not_empty";
    //public const string IS_NULL_OR_WHITE_SPACE = "is_null_or_white_space";
    //public const string IS_NOT_NULL_OR_WHITE_SPACE = "is_not_null_or_white_space";

    public const string NONE = "none";


    //-----------------------------//

    /// <summary>
    /// All the FilterTypes that can be applied to dates
    /// </summary>
    public static readonly HashSet<string> DATE_TYPES =
      [
       EQUALS,
       NOT_EQUAL_TO,
       GREATER_THAN,
       GREATER_THAN_OR_EQUAL_TO,
       LESS_THAN,
       LESS_THAN_OR_EQUAL_TO,
       BETWEEN,
       IN
      ];

    /// <summary>
    /// All the FilterTypes that can be applied to strings
    /// </summary>
    public static readonly HashSet<string> STRING_TYPES =
      [
       EQUALS,
       NOT_EQUAL_TO,
       CONTAINS,
       STARTS_WITH,
       ENDS_WITH,
       IN
      ];

    /// <summary>
    /// All the FilterTypes that can be applied to numbers
    /// </summary>
    public static readonly HashSet<string> NUMBER_TYPES =
      [
       EQUALS,
       NOT_EQUAL_TO,
       GREATER_THAN,
       GREATER_THAN_OR_EQUAL_TO,
       LESS_THAN,
       LESS_THAN_OR_EQUAL_TO,
       BETWEEN,
       IN
      ];

    /// <summary>
    /// All the FilterTypes that can be applied to bools
    /// </summary>
    public static readonly HashSet<string> BOOLEAN_TYPES =
        [
        EQUALS,
        NOT_EQUAL_TO
        ];

    //- - - - - - - - - - - - - - -//

    private static readonly Dictionary<string, ExpressionType> _filterTypeToExpressionTypeMapNumeric = new() {
     {EQUALS,ExpressionType.Equal},
     {NOT_EQUAL_TO,ExpressionType.NotEqual},
     {GREATER_THAN,ExpressionType.GreaterThan},
     {GREATER_THAN_OR_EQUAL_TO,ExpressionType.GreaterThanOrEqual},
     {LESS_THAN,ExpressionType.LessThan},
     {LESS_THAN_OR_EQUAL_TO,ExpressionType.LessThanOrEqual}
  };

    //- - - - - - - - - - - - - - -//

    private static readonly Dictionary<string, ExpressionType> _filterTypeToExpressionTypeMapDate = new() {
     {EQUALS,ExpressionType.Equal},
     {NOT_EQUAL_TO,ExpressionType.NotEqual},
     {GREATER_THAN,ExpressionType.GreaterThan},
     {GREATER_THAN_OR_EQUAL_TO,ExpressionType.GreaterThanOrEqual},
     {LESS_THAN,ExpressionType.LessThan},
     {LESS_THAN_OR_EQUAL_TO,ExpressionType.LessThanOrEqual}
  };

    //- - - - - - - - - - - - - - -//

    private static readonly Dictionary<string, ExpressionType> _filterTypeToExpressionTypeMapEnum = new() {
     {EQUALS,ExpressionType.Equal},
     {NOT_EQUAL_TO,ExpressionType.NotEqual},
     {GREATER_THAN,ExpressionType.GreaterThan},
     {GREATER_THAN_OR_EQUAL_TO,ExpressionType.GreaterThanOrEqual},
     {LESS_THAN,ExpressionType.LessThan},
     {LESS_THAN_OR_EQUAL_TO,ExpressionType.LessThanOrEqual}
  };

    //-----------------------------//

    internal static PgResult<ExpressionType> GetNumericExpressionType(string filterType)
    {

        if (_filterTypeToExpressionTypeMapNumeric.TryGetValue(filterType, out ExpressionType value))
            return new PgResult<ExpressionType>(value);

        return new PgResult<ExpressionType>(false, $"FilterType, {filterType} not found in Numeric expression types");

    }

    //-----------------------------//

    internal static PgResult<ExpressionType> GetDateExpressionType(string filterType)
    {

        if (_filterTypeToExpressionTypeMapDate.TryGetValue(filterType, out ExpressionType value))
            return new PgResult<ExpressionType>(value);

        return new PgResult<ExpressionType>(false, $"FilterType, {filterType} not found in Date expression types");

    }

    //-----------------------------//

    internal static PgResult<ExpressionType> GetEnumExpressionType(string filterType)
    {

        if (_filterTypeToExpressionTypeMapEnum.TryGetValue(filterType, out ExpressionType value))
            return new PgResult<ExpressionType>(value);

        return new PgResult<ExpressionType>(false, $"FilterType, {filterType} not found in Enum expression types");

    }

    //-----------------------------//

}//Cls
