using System.Reflection;

namespace ID.Tests.Utility;
public class InterfaceScanner
{
    public static List<Type> GetApplicationInterfaces(Assembly applicationAssembly)
    {
        // Get all interface types defined in the assembly
        var interfaces = applicationAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Namespace != null && t.Namespace.StartsWith("ID.Application"))
            .ToList();

        // Example: Print all interface names
        foreach (var iface in interfaces)
        {
            Console.WriteLine(iface.FullName);
        }

        return interfaces;
    }





}
