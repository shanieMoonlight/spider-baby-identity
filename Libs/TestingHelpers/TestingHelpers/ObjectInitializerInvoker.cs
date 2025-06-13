using System.Reflection;
using System.Text;

namespace TestingHelpers;

//==========================================================//

public class ObjectInitializerInvoker
{

    public static T InitializeObject<T>(T obj, params PropertyAssignment[] assignments) where T : class
    {
        foreach (var assignment in assignments)
        {
            PropertyInfo? propertyInfo = typeof(T).GetProperty(assignment.PropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (propertyInfo != null)
            {
                if (propertyInfo.CanWrite)
                    propertyInfo.SetValue(obj, assignment.GetValue());
                else
                    SetPrivateProperty(obj, propertyInfo, assignment.GetValue());
            }
            else
            {
                SetFieldInfo(obj, assignment);
            }
        }
        return obj;
    }

    //- - - - - - - - - - - - - - - - - - //  

    private static void SetPrivateProperty<T>(T obj, PropertyInfo propertyInfo, object? value) where T : class
    {

        MethodInfo? setter = propertyInfo.GetSetMethod(true);
        if (setter == null)
        {
            // Traverse the inheritance hierarchy to find the setter in the parent class
            Type? baseType = typeof(T).BaseType;
            while (baseType != null && setter == null)
            {
                PropertyInfo? basePropertyInfo = baseType.GetProperty(propertyInfo.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                if (basePropertyInfo != null)
                    setter = basePropertyInfo.GetSetMethod(true);
                baseType = baseType.BaseType;
            }
        }

        if (setter != null)
        {
            setter.Invoke(obj, [value]);
            return;
        }

        //Try setting backing field
        FieldInfo? fieldInfo = FindBackingField(propertyInfo, propertyInfo.Name);
        fieldInfo?.SetValue(obj, value);
        return;
    }

    //- - - - - - - - - - - - - - - - - - //  

    private static T SetFieldInfo<T>(T obj, PropertyAssignment assignment) where T : class
    {
        FieldInfo? fieldInfo = typeof(T).GetField(assignment.PropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        fieldInfo?.SetValue(obj, assignment.GetValue());
        return obj;
    }

    //- - - - - - - - - - - - - - - - - - //  


    public static FieldInfo? FindBackingField(PropertyInfo propertyInfo, string propertyName)
    {
        if (propertyInfo == null || string.IsNullOrEmpty(propertyName))
            return null;

        // Try to find the auto-generated backing field
        var backingFieldName = $"<{propertyName}>k__BackingField";
        var backingField = propertyInfo.DeclaringType?.GetField(backingFieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (backingField != null)
            return backingField;

        // Analyze the getter method
        var getterMethod = propertyInfo.GetGetMethod(true);
        if (getterMethod != null)
        {
            var bodyInstructions = getterMethod.GetMethodBody()?.GetILAsByteArray();
            if (bodyInstructions != null)
            {
                for (int i = 0; i < bodyInstructions.Length; i++)
                {
                    if (bodyInstructions[i] == 0x7E && i + 1 < bodyInstructions.Length && bodyInstructions[i + 1] == 0x04)
                    {
                        var fieldName = Encoding.UTF8.GetString(bodyInstructions, i + 2, bodyInstructions.Length - i - 2);
                        return propertyInfo.DeclaringType?.GetField(fieldName,
                            BindingFlags.NonPublic | BindingFlags.Instance);
                    }
                }
            }
        }

        // Check all private fields against the setter method
        var setterMethod = propertyInfo.GetSetMethod(true);
        if (setterMethod != null)
        {
            var bodyInstructions = setterMethod.GetMethodBody()?.GetILAsByteArray();
            if (bodyInstructions != null)
            {
                foreach (var field in propertyInfo.DeclaringType?.GetFields(BindingFlags.NonPublic | BindingFlags.Instance) ?? [])
                {
                    for (int i = 0; i < bodyInstructions.Length; i++)
                    {
                        if (bodyInstructions[i] == 0x04 &&
                            i + 1 < bodyInstructions.Length &&
                            Encoding.UTF8.GetString(bodyInstructions, i + 1, bodyInstructions.Length - i - 1) == field.Name)
                        {
                            return field;
                        }
                    }
                }
            }
        }

        return null;
    }

}

//==========================================================//

public class PropertyAssignment(string propertyName, Func<object?> getValue)
{
    public string PropertyName { get; set; } = propertyName;
    public Func<object?> GetValue { get; set; } = getValue;
}

//==========================================================//