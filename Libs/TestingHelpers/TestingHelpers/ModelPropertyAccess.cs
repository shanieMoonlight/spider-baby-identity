using System.Reflection;

namespace TestingHelpers;

public static class ModelPropertyAccess
{


    public static T? GetValue<T>(object obj, string propertyName)
    {
        PropertyInfo? propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return (T?)propertyInfo?.GetValue(obj);
    }

    //--------------------------//

    public static void SetValue<T>(object obj, string propertyName, T value)
    {
        PropertyInfo? propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        propertyInfo?.SetValue(obj, value);
    }

}

