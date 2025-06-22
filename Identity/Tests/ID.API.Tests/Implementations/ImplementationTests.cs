using ID.Application;
using ID.Infrastructure;
using ID.Tests.Utility;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Diagnostics;

namespace ID.API.Tests.Implementations;

public class ImplementationTests
{

    [Fact]
    public void ShouldImplementAllApplicationServices()
    {
        // Get all interfaces in ID.Application
        var appAssembly = IdApplicationAssemblyReference.Assembly;
        var infraAssembly = IdInfrastructureAssemblyReference.Assembly;

        var interfaces = InterfaceScanner.GetApplicationInterfaces(appAssembly);

        interfaces.ShouldNotBeEmpty("No interfaces found in the application assembly.");

        // Get all concrete classes in both assemblies
        var allTypes = appAssembly.GetTypes()
            .Concat(infraAssembly.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        // For each interface, check if implemented in either namespace
        var notImplemented = interfaces
       .Where(iface =>
       {
           if (!iface.IsGenericTypeDefinition)
           {
               // Non-generic interface: use IsAssignableFrom as before
               return !allTypes.Any(t => iface.IsAssignableFrom(t));
           }
           else
           {
               // Open generic interface: check if any class implements a closed version
               return !allTypes.Any(t =>
                   t.GetInterfaces().Any(i =>
                       i.IsGenericType &&
                       i.GetGenericTypeDefinition() == iface
                   )
               );
           }
       })
       .ToList();

        foreach (var iface in notImplemented)
        {
            Debug.WriteLine(iface.FullName);
        }

        notImplemented.ShouldBeEmpty($"The following interfaces are not implemented: {string.Join(", ", notImplemented.Select(i => Environment.NewLine + i.FullName))}");
    }

    //------------------------------------//


    //[Fact]
    //public void All_Implementations_Should_Be_Registered_For_DI()
    //{
    //    // Arrange: Build the service collection as in your app
    //    var services = new ServiceCollection();
    //    // Call your actual registration method here:
    //    // e.g., services.AddMyAppServices();
    //    // or mimic your Startup.cs registration logic

    //    // Build the provider
    //    var provider = services.BuildServiceProvider();

    //    // Get all interfaces
    //    var appAssembly = IdApplicationAssemblyReference.Assembly;
    //    var infraAssembly = IdInfrastructureAssemblyReference.Assembly;
    //    var interfaces = InterfaceScanner.GetApplicationInterfaces(appAssembly);

    //    // Get all concrete classes implementing those interfaces
    //    var allTypes = appAssembly.GetTypes()
    //        .Concat(infraAssembly.GetTypes())
    //        .Where(t => t.IsClass && !t.IsAbstract)
    //        .ToList();

    //    var notRegistered = interfaces
    //        .SelectMany(iface =>
    //            allTypes.Where(t =>
    //                !iface.IsGenericTypeDefinition
    //                    ? iface.IsAssignableFrom(t)
    //                    : t.GetInterfaces().Any(i =>
    //                        i.IsGenericType &&
    //                        i.GetGenericTypeDefinition() == iface
    //                    )
                    
    //            )
    //            .Select(impl => (iface, impl))
    //        )
    //        .Where(pair =>
    //        {
    //            // Try to resolve the implementation from the provider
    //            var service = provider.GetService(pair.iface);
    //            return service == null || !pair.impl.IsInstanceOfType(service);
    //        })
    //        .ToList();

    //    notRegistered.ShouldBeEmpty($"The following implementations are not registered for DI: {string.Join(", ", notRegistered.Select(nr => $"{nr.impl.FullName} for {nr.iface.FullName}"))}");
    //}


}//Cls
