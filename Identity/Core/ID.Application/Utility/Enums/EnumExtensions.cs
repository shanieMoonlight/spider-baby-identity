using System.ComponentModel;
using System.Reflection;

namespace ID.Application.Utility.Enums;

public static class EnumExtensions
{

    //---------------------------------//

    public static string? GetDescription(this Enum value, string? fallbackValue = null)
    {
        Type type = value.GetType();
        string? name = Enum.GetName(type, value);

        if (name == null)
            return fallbackValue;

        FieldInfo? field = type.GetField(name);

        if (field is null)
            return fallbackValue;

        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
            return attr.Description;

        return fallbackValue;
    }

    //---------------------------------//


    public static int ToInt<TEnum>(this TEnum tenum) where TEnum : Enum =>
        Convert.ToInt32(tenum);

    //---------------------------------//


}//Cls
