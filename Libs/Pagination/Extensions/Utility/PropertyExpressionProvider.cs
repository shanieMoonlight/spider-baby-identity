using ExpressionHelpers;
using StringHelpers;
using System.Linq.Expressions;

namespace Pagination.Extensions.Utility;
internal class PropertyExpressionProvider
{    /// <summary>
    /// Creates a property expression from a ParamaterExpression and a string 
    /// </summary>
    /// <param name="param">the ' part</param>
    /// <param name="field">the property name/description</param>
    /// <param name="getPropertySelectorLambda">Converts the field to the appropriate name for the property</param>
    internal static MemberExpression GetPropertyExpression(ParameterExpression param, string field, Func<string, string>? getPropertySelectorLambda = null)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException($"{nameof(GetPropertyExpression)}: Field name cannot be empty or whitespace", nameof(field));
            
        string propertyName = getPropertySelectorLambda != null
            ? getPropertySelectorLambda.Invoke(field)
            : field.CamelToPascal();
            
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException($"{nameof(GetPropertyExpression)}: Property name resolved to empty for field '{field}'", nameof(field));

        return param.ToMemberExpression(propertyName); //x.Description
    }


}
