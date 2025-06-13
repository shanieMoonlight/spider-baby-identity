using System.Linq;
using System.Reflection;
using ID.Infrastructure.Setup;
using System.Runtime.CompilerServices;

namespace ID.Infrastructure.Tests.Setup;

public class IdInfrastructureSetupOptionsTests
{
    [Fact]
    public void AllPublicProperties_ShouldHaveRequiredModifier()
    {
        // Arrange
        var type = typeof(IdInfrastructureSetupOptions);

        // Act
        var publicProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertiesWithoutRequiredModifier = publicProperties
            .Where(prop => !prop.CustomAttributes.Any(attr => attr.AttributeType.Name == nameof(RequiredMemberAttribute)))
            .Select(prop => prop.Name)
            .ToList();

        // Assert
        Assert.Empty(propertiesWithoutRequiredModifier);
    }
}