using System.Reflection;

namespace TestingHelpers;

public static class NonPublicClassMembers
{
    //------------------------------------//

    public static FieldType GetField<T, FieldType>(T instance, string fieldName) where FieldType : class =>
        typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(instance) as FieldType
         ?? throw new ArgumentException($"Field '{fieldName}' not found.");

    //- - - - - - - - - - - - - - - - - - //

    public static void SetField<T>(T instance, string fieldName, object newValue)
    {
        var field = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
            field.SetValue(instance, newValue);
        else
            throw new ArgumentException($"Field '{fieldName}' not found.");
    }

    //------------------------------------//

    public static MethodInfo GetMethod<T>(string methodName) =>
         typeof(T).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)
         ?? throw new ArgumentException($"Method '{methodName}' not found.");

    //- - - - - - - - - - - - - - - - - - //

    public static void InvokeMethod<T>(T instance, string methodName, object?[]? parameters) =>
        GetMethod<T>(methodName)
            .Invoke(instance, parameters);

    //------------------------------------//

}//Cls
