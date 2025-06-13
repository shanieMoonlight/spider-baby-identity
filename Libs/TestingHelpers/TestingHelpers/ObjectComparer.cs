using System.Reflection;

namespace TestingHelpers;

public static class ObjectComparer
{
    public static bool AreEqual(object obj1, object obj2)
    {
        if (ReferenceEquals(obj1, obj2))
            return true;

        if (obj1 == null || obj2 == null)
            return false;

        Type type = obj1.GetType();
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            object? value1 = property.GetValue(obj1);
            object? value2 = property.GetValue(obj2);

            if (!Equals(value1, value2))
                return false;
        }

        return true;
    }

}