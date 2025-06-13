using ID.Application;
using ID.Application.Jobs.Abstractions;
using ID.Infrastructure;

namespace ID.Application.Tests.Jobs;

public class JobHandlerImplementationTests
{
    [Fact]
    public void AllAbstractJobHandlersShouldHaveImplementationsInInfrastructure()
    {
        // Load the assemblies
        var applicationAssembly = IdApplicationAssemblyReference.Assembly;
        var infrastructureAssembly = IdInfrastructureAssemblyReference.Assembly;

        // Find all abstract classes in ID.Application that extend AMyIdJobHandler
        var abstractJobHandlers = applicationAssembly.GetTypes()
            .Where(t => t.IsAbstract && t.IsSubclassOf(typeof(AMyIdJobHandler)))
            .ToList();

        // Check if each abstract class has an implementation in ID.Infrastructure
        foreach (var abstractJobHandler in abstractJobHandlers)
        {
            var implementations = infrastructureAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && abstractJobHandler.IsAssignableFrom(t))
                .ToList();

            Assert.True(implementations.Count != 0, $"No implementation found for {abstractJobHandler.FullName}");
        }
    }
}
