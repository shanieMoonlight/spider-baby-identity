using ID.IntegrationEvents.Abstractions;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;

namespace ID.IntegrationEvents.Tests;

public class IntegrationEventTests(ITestOutputHelper output)
{
    [Fact]
    public void AllIntegrationEvents_ShouldHaveParameterlessConstructor()
    {
        var integrationEventType = typeof(IIdIntegrationEvent);
        var types = GetAllIntegrationEventTypes();

        foreach (var type in types)
        {
            var constructors = type.GetConstructors();
            bool hasParameterlessConstructor = constructors.Any(c => c.GetParameters().Length == 0);

            Assert.True(hasParameterlessConstructor, $"{type.FullName} does not have a parameterless constructor. It's required for Serialization");
        }
    }

    //-----------------------//

    [Fact]
    public void AllIntegrationEvents_ShouldHaveRequiredProperties()
    {
        var integrationEventTypes = GetAllIntegrationEventTypes();

        foreach (var type in integrationEventTypes)
        {
            // Ensure they have timestamp, correlation ID, etc
            output.WriteLine($"Checking properties for {type.FullName} - {HasProperty(type, "OccurredAtUtc")}");
            Assert.True(HasProperty(type, "OccurredAtUtc"),
                $"{type.FullName} should have a timestamp property");
        }
    }

    //-----------------------//

    private static IEnumerable<Type> GetAllIntegrationEventTypes()
    {
        var integrationEventType = typeof(IIdIntegrationEvent);
        return IdIntegrationEventsAssemblyReference.Assembly.GetTypes()
            .Where(t => integrationEventType.IsAssignableFrom(t) 
                        && !t.IsInterface 
                        && !t.IsAbstract);
    }

    private static bool HasProperty(Type type, string propertyName) => 
        type.GetProperties().Any(p => p.Name == propertyName);

}//Cls