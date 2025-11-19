using ID.OAuth.Utils.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace ID.OAuth.Utils.Tests.Setup;

public class ServiceCollectionReflectionTests
{
    [Fact]
    public void AddMyIdOAuthUtils_RegistersImplementationsForAllLocalInterfaces()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - register with action overload
        services.AddMyIdOAuthUtils();

        // Get the Amazon assembly that contains the setup types
        var asm = IdOAuthUtilsAssemblyReference.Assembly;

        // Find interfaces declared in the Amazon assembly and under the Amazon namespace
        var interfaces = asm.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic && t.Namespace != null && t.Namespace.StartsWith("ID.OAuth.Utils"))
            .ToList();

        // For each interface check if there is a service descriptor that matches or a closed generic registered
        var missing = new List<string>();

        foreach (var iface in interfaces)
        {
            bool found = services.Any(sd =>
            {
                // Direct service type match
                if (sd.ServiceType == iface)
                    return true;

                // Service registered as closed generic where the generic definition matches
                if (iface.IsGenericTypeDefinition && sd.ServiceType.IsGenericType)
                {
                    if (sd.ServiceType.GetGenericTypeDefinition() == iface)
                        return true;
                }

                // Implementation type assignable to interface
                if (sd.ImplementationType != null && iface.IsAssignableFrom(sd.ImplementationType))
                    return true;

                // Implementation instance known
                if (sd.ImplementationInstance != null && iface.IsAssignableFrom(sd.ImplementationInstance.GetType()))
                    return true;

                return false;
            });

            if (!found)
                missing.Add(iface.FullName ?? iface.Name);
        }

        // Assert - no missing registrations
        missing.Count.ShouldBe(0, $"The following interfaces declared in the Amazon assembly were not registered: {string.Join(", ", missing)}");
    }

    //----------------------//


    [Fact]
    public void AddMyIdOAuthUtils_RegistersJsonSerializerOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdOAuthUtils();

        // Assert - registration exists in collection
        services.Any(sd => sd.ServiceType == typeof(JsonSerializerOptions)).ShouldBeTrue();

        // And it resolves with expected settings
        var sp = services.BuildServiceProvider();
        var jsonOpts = sp.GetRequiredService<JsonSerializerOptions>();
        jsonOpts.PropertyNameCaseInsensitive.ShouldBeTrue();
        jsonOpts.AllowTrailingCommas.ShouldBeTrue();
    }

}//Cls
