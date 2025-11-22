using System.Reflection;

namespace TestingHelpers.Reflection;

//##########################################################################//

public static class PrivatePropertyUpdater
{
    public static T UpdateProperties<T>(T obj, params PropertyAssignment[] props) where T : class
    {
        Type type = typeof(T);

        foreach (var prop in props)
        {
            var propertyInfo = type.GetProperty(prop.PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? throw new ArgumentException($"Property '{prop.PropertyName}' not found on type '{type.Name}'.");
            propertyInfo.SetValue(obj, prop.GetValue());
        }

        return obj;
    }
}//Cls
