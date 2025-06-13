using StringHelpers;
using System.Linq.Expressions;

namespace ExpressionHelpers;

public static class ExpressionExtensions
{
    /// <summary>
    /// Converts <paramref name="param"/> into a MemberExpression with <paramref name="propertyName"/> as the field
    /// </summary>
    /// <param name="param">The parameter (Object with <paramref name="propertyName"/> as a field)</param>
    /// <param name="propertyName">The field on <paramref name="param"/></param>
    /// <returns>The expression equivalent of param.PropertyName </returns>
    public static MemberExpression ToMemberExpression(this ParameterExpression param, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException("Property name cannot be empty or whitespace", nameof(propertyName));

        Expression body = param;
        foreach (var member in propertyName.CamelToPascal().Split('.'))
        {
            body = Expression.PropertyOrField(body, member);
        }
        return (MemberExpression)body;
    }

    //-----------------------------//

    /// <summary>
    /// Creates a conditional expression that safely handles null references when accessing properties.
    /// Similar to the C# null-conditional operator (?.)
    /// </summary>
    /// <param name="o">The expression to check for null</param>
    /// <param name="property">The name of the property to access</param>
    /// <returns>An expression that returns null if <paramref name="o"/> is null, otherwise returns the property value</returns>
    /// <remarks>
    /// For value types, this method wraps the result in a Nullable&lt;T&gt; to allow null propagation.
    /// </remarks>
    public static Expression CreateNullPropagationExpression(this Expression o, string property)
    {
        if (string.IsNullOrWhiteSpace(property))
            throw new ArgumentException("Property name cannot be empty or whitespace", nameof(property));

        Expression propertyAccess = Expression.Property(o, property);

        var propertyType = propertyAccess.Type;

        if (propertyType.IsValueType && Nullable.GetUnderlyingType(propertyType) == null)
            propertyAccess = Expression.Convert(
                propertyAccess, typeof(Nullable<>).MakeGenericType(propertyType));

        var nullResult = Expression.Default(propertyAccess.Type);

        var condition = Expression.Equal(o, Expression.Constant(null, o.Type));

        return Expression.Condition(condition, nullResult, propertyAccess);
    }

    //-----------------------------//

    /// <summary>
    /// Determines whether an expression represents a nullable value type.
    /// </summary>
    /// <param name="exp">The expression to check</param>
    /// <returns>
    /// True if the expression type is a nullable value type (Nullable&lt;T&gt;);
    /// false otherwise
    /// </returns>
    /// <remarks>
    /// This only checks for nullable value types, not reference types which are implicitly nullable.
    /// </remarks>
    public static bool IsNullable(this Expression exp)
        => Nullable.GetUnderlyingType(exp.Type) != null;
}