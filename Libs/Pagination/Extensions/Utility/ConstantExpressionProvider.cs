using Pagination.Result;
using System.Linq.Expressions;

namespace Pagination.Extensions.Utility;

internal class ConstantExpressionProvider
{

    /// <summary>
    /// Crates a ConstantExpression of Type <paramref name="type"/> by parsing <paramref name="str"/>
    /// and wraps it in a PhResult
    /// </summary>
    /// <param name="type">Type of number</param>
    /// <param name="str">Number as string</param>
    /// <returns>GenrRsult of ConstantExpression</returns>
    internal static PgResult<ConstantExpression> CreateNumericConstantExpression(Type type, string str)
    {
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Double => TryCreateConstantExpression<double>(double.TryParse, str, type),
            TypeCode.Int32 => TryCreateConstantExpression<int>(int.TryParse, str, type),
            TypeCode.Int16 => TryCreateConstantExpression<short>(short.TryParse, str, type),
            TypeCode.Int64 => TryCreateConstantExpression<long>(long.TryParse, str, type),
            TypeCode.Decimal => TryCreateConstantExpression<decimal>(decimal.TryParse, str, type),
            TypeCode.Byte => TryCreateConstantExpression<byte>(byte.TryParse, str, type),
            TypeCode.UInt16 => TryCreateConstantExpression<ushort>(ushort.TryParse, str, type),
            TypeCode.UInt32 => TryCreateConstantExpression<uint>(uint.TryParse, str, type),
            TypeCode.UInt64 => TryCreateConstantExpression<ulong>(ulong.TryParse, str, type),
            TypeCode.SByte => TryCreateConstantExpression<sbyte>(sbyte.TryParse, str, type),
            TypeCode.Single => TryCreateConstantExpression<float>(float.TryParse, str, type),
            _ => PgResult<ConstantExpression>.Failure($"Unsupported numeric type: {type.FullName}. This method only supports standard .NET numeric types. Do you need to add \".Value\" to your getPropertySelectorLambda return value, to handle nullable types")
        };
    }

    //-----------------------------------// 

    internal static PgResult<ConstantExpression> CreateNumericListConstantExpression(Type type, string[] numStrings)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        var isNullableType = underlyingType != null;


        if (!isNullableType)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Double => TryCreateNumericListConstantExpression<double>(double.TryParse, numStrings, type),
                TypeCode.Int32 => TryCreateNumericListConstantExpression<int>(int.TryParse, numStrings, type),
                TypeCode.Int16 => TryCreateNumericListConstantExpression<short>(short.TryParse, numStrings, type),
                TypeCode.Int64 => TryCreateNumericListConstantExpression<long>(long.TryParse, numStrings, type),
                TypeCode.Decimal => TryCreateNumericListConstantExpression<decimal>(decimal.TryParse, numStrings, type),
                TypeCode.Byte => TryCreateNumericListConstantExpression<byte>(byte.TryParse, numStrings, type),
                TypeCode.UInt16 => TryCreateNumericListConstantExpression<ushort>(ushort.TryParse, numStrings, type),
                TypeCode.UInt32 => TryCreateNumericListConstantExpression<uint>(uint.TryParse, numStrings, type),
                TypeCode.UInt64 => TryCreateNumericListConstantExpression<ulong>(ulong.TryParse, numStrings, type),
                TypeCode.SByte => TryCreateNumericListConstantExpression<sbyte>(sbyte.TryParse, numStrings, type),
                TypeCode.Single => TryCreateNumericListConstantExpression<float>(float.TryParse, numStrings, type),
                _ => PgResult<ConstantExpression>.Failure("Type not found"),
            };
        }


        return Type.GetTypeCode(underlyingType) switch
        {
            TypeCode.Double => TryCreateNumericListConstantExpressionNullable<double>(double.TryParse, numStrings, type),
            TypeCode.Int32 => TryCreateNumericListConstantExpressionNullable<int>(int.TryParse, numStrings, type),
            TypeCode.Int16 => TryCreateNumericListConstantExpressionNullable<short>(short.TryParse, numStrings, type),
            TypeCode.Int64 => TryCreateNumericListConstantExpressionNullable<long>(long.TryParse, numStrings, type),
            TypeCode.Decimal => TryCreateNumericListConstantExpressionNullable<decimal>(decimal.TryParse, numStrings, type),
            TypeCode.Byte => TryCreateNumericListConstantExpressionNullable<byte>(byte.TryParse, numStrings, type),
            TypeCode.UInt16 => TryCreateNumericListConstantExpressionNullable<ushort>(ushort.TryParse, numStrings, type),
            TypeCode.UInt32 => TryCreateNumericListConstantExpressionNullable<uint>(uint.TryParse, numStrings, type),
            TypeCode.UInt64 => TryCreateNumericListConstantExpressionNullable<ulong>(ulong.TryParse, numStrings, type),
            TypeCode.SByte => TryCreateNumericListConstantExpressionNullable<sbyte>(sbyte.TryParse, numStrings, type),
            TypeCode.Single => TryCreateNumericListConstantExpressionNullable<float>(float.TryParse, numStrings, type),
            _ => PgResult<ConstantExpression>.Failure("Type not found"),
        };

    }

    //-----------------------------------// 

    /// <summary>
    /// Converts a list of strings into a list of O and then turns that into a CnstantExpression
    /// </summary>
    /// <typeparam name="Output">Type of number</typeparam>
    /// <param name="tryParser">Specific parser (i.e double.TryParse</param>
    /// <param name="numStrings">List of numeric strings</param>
    /// <param name="type">Type</param>
    /// <returns>ConstantExpression in a PhResult</returns>
    internal static PgResult<ConstantExpression> TryCreateNumericListConstantExpression<Output>(TryParseDelegate<Output> tryParser, string[] numStrings, Type type)
    {

        var newList = new List<Output>(numStrings.Length);

        //Cast all strings to Os
        foreach (var numStr in numStrings)
        {
            if (tryParser(numStr, out var outVar))
                newList.Add(outVar);
            else
                return PgResult<ConstantExpression>.Failure($"Could not parse '{string.Join(", ", numStrings)}' as type {type.Name}");
        }

        return new(Expression.Constant(newList));
    }

    //-----------------------------------//  

    /// <summary>
    /// Converts a list of strings into a list of O and then turns that into a ConstantExpression
    /// </summary>
    /// <typeparam name="Output">Type of number</typeparam>
    /// <param name="tryParser">Specific parser (i.e double.TryParse</param>
    /// <param name="numStrings">List of numeric strings</param>
    /// <param name="type">Type</param>
    /// <returns>ConstantExpression in a PhResult</returns>
    internal static PgResult<ConstantExpression> TryCreateNumericListConstantExpressionNullable<Output>(TryParseDelegate<Output> tryParser, string[] numStrings, Type type) where Output : struct
    {
        var newList = new List<Output>(numStrings.Length);

        //Cast all strings to Os
        foreach (var numStr in numStrings)
        {
            if (tryParser(numStr, out var outVar))
                newList.Add(outVar);
            else
                return PgResult<ConstantExpression>.Failure($"{numStrings} is not a type of {type.Name}");
        }

        return new PgResult<ConstantExpression>(Expression.Constant(newList));
    }

    //-----------------------------------//  

    /// <summary>
    /// Try to parse a string into a number and return result wrapped in a PhResult
    /// </summary>
    /// <typeparam name="Output">Type of number</typeparam>
    /// <param name="tryParser">Specific parser (i.e double.TryParse</param>
    /// <param name="numStr">Number in string form</param>
    /// <param name="type">Type</param>
    /// <returns>ConstantExpression in a PhResult</returns>
    internal static PgResult<ConstantExpression> TryCreateConstantExpression<Output>(TryParseDelegate<Output> tryParser, string numStr, Type type)
    {

        if (tryParser(numStr, out var outVar))
            return new(Expression.Constant(outVar, typeof(Output)));
        else
            return PgResult<ConstantExpression>.Failure($"{numStr} is not a type of {type.Name}");

    }


    //- - - - - - - - - - - - - - - - - -//

    internal delegate bool TryParseDelegate<Output>(string input, out Output output);
}
