using ExpressionHelpers;
using System.Linq.Expressions;

namespace Pagination.Extensions.Utility;
internal class NullabilityHandler
{
    /// <summary>
    /// Makes sure that nullable type expressions include '.Value' 
    /// </summary>
    /// <typeparam name="T">Nullable type (DateTime, int, etc)</typeparam>
    /// <param name="exp">Potentially nullable property</param>
    /// <returns>Sanitized Expression</returns>
    internal static MemberExpression HandleNullability<T>(MemberExpression exp) where T : struct =>
        !exp.IsNullable() ? exp : Expression.PropertyOrField(exp, "Value");

    //-----------------------------------//

    /// <summary>
    /// If Expression is of nullable type adds Hasvalue check expression before it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="originalPropertyExp">Potentially nullable property</param>
    /// <param name="exp">Expression derived from <paramref name="originalPropertyExp"/></param>
    /// <returns>Sanitized Expression</returns>
    internal static Expression HandleHasValue<T>(MemberExpression originalPropertyExp, Expression exp) where T : struct
    {
        if (!originalPropertyExp.IsNullable())
            return exp;

        var hasValueExp = Expression.Property(originalPropertyExp, "HasValue");
        return Expression.AndAlso(hasValueExp, exp);
    }

}
