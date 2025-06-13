namespace Pagination.Extensions.Utility;
internal class NumericEnumerableTypeProvider
{
    /// <summary>
    /// Crates a Type of Numeric IEnumerable <paramref name="type"/> if <paramref name="type"/> is also numeric
    /// </summary>
    /// <param name="type">Type of number</param>
    /// <returns>Type of Numeric IEnumerable or null</returns>
    internal static Type? GetIEnumerableType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        var isNullableType = underlyingType != null;

        if (isNullableType)
            return GetIEnumerableTypeNullable(underlyingType);

        return GetIEnumerableTypeNonNullable(type);
    }

    //-----------------------------------//  

    /// <summary>
    /// Crates a Type of NonNullable Numeric IEnumerable <paramref name="type"/> if <paramref name="type"/> is also numeric
    /// and wraps it in a PhResult
    /// </summary>
    /// <param name="type">Type of number</param>
    /// <returns>Type of Numeric IEnumerable or null</returns>
    internal static Type? GetIEnumerableTypeNonNullable(Type? type)
    {
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Double => typeof(List<double>),
            TypeCode.Int32 => typeof(List<int>),
            TypeCode.Int16 => typeof(List<short>),
            TypeCode.Int64 => typeof(List<long>),
            TypeCode.Decimal => typeof(List<decimal>),
            TypeCode.Byte => typeof(List<byte>),
            TypeCode.UInt16 => typeof(List<ushort>),
            TypeCode.UInt32 => typeof(List<uint>),
            TypeCode.UInt64 => typeof(List<ulong>),
            TypeCode.SByte => typeof(List<sbyte>),
            TypeCode.Single => typeof(List<float>),
            _ => null,
        };
    }

    //-----------------------------------//  

    /// <summary>
    /// Crates a Type of Nullable Numeric IEnumerable <paramref name="type"/> if <paramref name="type"/> is also numeric
    /// </summary>
    /// <param name="type">Type of number</param>
    /// <returns>Type of Numeric IEnumerable or null</returns>
    internal static Type? GetIEnumerableTypeNullable(Type? type)
    {
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Double => typeof(List<double?>),
            TypeCode.Int32 => typeof(List<int?>),
            TypeCode.Int16 => typeof(List<short?>),
            TypeCode.Int64 => typeof(List<long?>),
            TypeCode.Decimal => typeof(List<decimal?>),
            TypeCode.Byte => typeof(List<byte?>),
            TypeCode.UInt16 => typeof(List<ushort?>),
            TypeCode.UInt32 => typeof(List<uint?>),
            TypeCode.UInt64 => typeof(List<ulong?>),
            TypeCode.SByte => typeof(List<sbyte?>),
            TypeCode.Single => typeof(List<float?>),
            _ => null,
        };
    }


}//Cls
