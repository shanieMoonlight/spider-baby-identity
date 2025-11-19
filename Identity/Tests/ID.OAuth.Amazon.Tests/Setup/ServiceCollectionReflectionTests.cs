using FluentValidation;
using ID.OAuth.Amazon.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ID.OAuth.Amazon.Tests.Setup;

public class ServiceCollectionReflectionTests
{
    [Fact]
    public void AddMyIdAmazonOAuth_RegistersImplementationsForAllLocalInterfaces()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - register with action overload
        services.AddMyIdAmazonOAuth(opts =>
        {
            opts.ClientId = "cid";
            opts.ClientSecret = "secret";
            opts.ApiBaseUrl = "https://api.amazon.com/";
        });

        // Also ensure configuration overload registers similarly
        var inMemory = new Dictionary<string, string?>
        {
            ["AmazonOAuth:ClientId"] = "cfg-cid",
            ["AmazonOAuth:ClientSecret"] = "cfg-secret",
            ["AmazonOAuth:ApiBaseUrl"] = "https://api.amazon.com/"
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .Build();

        services.AddMyIdAmazonOAuth(config, "AmazonOAuth");

        // Get the Amazon assembly that contains the setup types
        var asm = typeof(IdOAuthAmazonOptions).Assembly;

        // Find interfaces declared in the Amazon assembly and under the Amazon namespace
        var interfaces = asm.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic && t.Namespace != null && t.Namespace.StartsWith("ID.OAuth.Amazon"))
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
    public void AddMyIdAmazonOAuth_ShouldRegisterValidators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdAmazonOAuth(opts =>
        {
            opts.ClientId = "cid";
            opts.ClientSecret = "secret";
            opts.ApiBaseUrl = "https://api.amazon.com/";
        });

        // Assert

        // Check if any validators are registered from the Customers assembly
        var validatorRegistrations = services
            .Where(sd => sd.ServiceType.IsGenericType &&
                   sd.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>))
            .Where(sd => sd.ImplementationType?.AssemblyQualifiedName != null && sd.ImplementationType.AssemblyQualifiedName.StartsWith("ID.OAuth.Amazon"))
            .ToList();

        // Make sure we have at least one validator registration
        validatorRegistrations.Count.ShouldBeGreaterThan(0, "No validators were registered");

    }

    //----------------------//

    [Fact]
    public void AddMyIdAmazonOAuth_RegistersJsonSerializerOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdAmazonOAuth(opts =>
        {
            opts.ClientId = "cid";
            opts.ClientSecret = "secret";
            opts.ApiBaseUrl = "https://api.amazon.com/";
        });

        // Assert - registration exists in collection
        services.Any(sd => sd.ServiceType == typeof(JsonSerializerOptions)).ShouldBeTrue();

        // And it resolves with expected settings
        var sp = services.BuildServiceProvider();
        var jsonOpts = sp.GetRequiredService<JsonSerializerOptions>();
        jsonOpts.PropertyNameCaseInsensitive.ShouldBeTrue();
        jsonOpts.AllowTrailingCommas.ShouldBeTrue();
    }

}//Cls
