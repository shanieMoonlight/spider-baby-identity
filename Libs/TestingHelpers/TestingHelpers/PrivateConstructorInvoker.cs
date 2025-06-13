using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace TestingHelpers;

//##########################################################################//

public static class PrivateConstructorInvoker
{
    public static T CreateNoParamsInstance<T>(params PropertyAssignment[] props) where T : class
    {
        Type type = typeof(T);

        ConstructorInfo? constructor = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(c => c.IsPrivate)
            ?? throw new ArgumentException("Private constructor not found");

        T t = (T)constructor.Invoke(null);
        return ObjectInitializerInvoker.InitializeObject(t, props);

    }


    public static bool ContainsNoParamsInstructor<T>() where T : class
    {
        Type type = typeof(T);
        return type
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(c => c.IsPrivate);
    }

}//Cls

//##########################################################################//

public static class NonPublicConstructorInvoker
{
    public static T CreateNoParamsInstance<T>(params PropertyAssignment[] props) where T : class
    {
        Type type = typeof(T);

        ConstructorInfo? constructor = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault()
            ?? throw new ArgumentException("NonPublic constructor not found");

        T t = (T)constructor.Invoke(null);
        return ObjectInitializerInvoker.InitializeObject(t, props);

    }


    public static bool ContainsNoParamsInstructor<T>() where T : class =>
        typeof(T)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .Length != 0;


}//Cls

//##########################################################################//

public static class PublicConstructorInvoker
{
    public static T CreateNoParamsInstance<T>(params PropertyAssignment[] props) where T : class
    {
        Type type = typeof(T);

        ConstructorInfo? constructor = type
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(c => c.GetParameters().Length == 0)
            ?? throw new ArgumentException("Public onstructor not found");

        T t = (T)constructor.Invoke(null);
        return ObjectInitializerInvoker.InitializeObject(t, props);

    }


    public static bool ContainsNoParamsInstructor<T>() where T : class
    {
        Type type = typeof(T);

        var what =  type
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Where(c => c.GetParameters().Length == 0);

        return type
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Any(c => c.GetParameters().Length == 0);
    }


}//Cls

//##########################################################################//

public static class ConstructorInvoker
{
    public static T CreateNoParamsInstance<T>(params PropertyAssignment[] props) where T : class
    {
        return PublicConstructorInvoker.ContainsNoParamsInstructor<T>()
            ? PublicConstructorInvoker.CreateNoParamsInstance<T>(props)
            : NonPublicConstructorInvoker.CreateNoParamsInstance<T>(props)
            ;

    }

}//Cls

//##########################################################################//