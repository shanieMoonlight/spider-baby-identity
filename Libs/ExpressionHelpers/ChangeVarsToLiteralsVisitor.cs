using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionHelpers;

/// <summary>
/// Expression visitor that evaluates member access on constant expressions,
/// converting them to literal constants in the expression tree.
/// </summary>
public class ChangeVarsToLiteralsVisitor : ExpressionVisitor
{

    /// <summary>
    /// Visits member expressions and simplifies them to constants when possible.
    /// </summary>
    protected override Expression VisitMember(MemberExpression memberExpression)
    {
        // Recurse down to see if we can simplify...
        var expression = Visit(memberExpression.Expression);

        // If we've ended up with a constant, and it's a property or a field,
        // we can simplify ourselves to a constant
        if (expression is ConstantExpression constExpression)
        {
            object? container = constExpression.Value;
            var member = memberExpression.Member;

            if (member is FieldInfo fieldInfo)
            {
                object? value = fieldInfo.GetValue(container);
                return Expression.Constant(value);
            }

            if (member is PropertyInfo propInfo)
            {
                object? value = propInfo.GetValue(container, null);
                return Expression.Constant(value);
            }

        }

        return base.VisitMember(memberExpression);

    }

}//Cls
