using FluentValidation;
using ID.OAuth.Google.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace ID.OAuth.Google.Tests.Setup;
public class IdOAuthGoogleSetupExtensionsTests
{
    private readonly IdOAuthGoogleOptions setupOptions = new()
    {
        ClientId = "356936492671-fokvkrpdd94Hfdimdi5av2dams76je2tl.apps.googleusercontent.com"
    };

    //------------------------------------//

    [Fact]
    public void AddIdApplication_ShouldRegisterValidators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdOAuthGoogle(setupOptions);

        // Assert

        // Check if any validators are registered from the Customers assembly
        var validatorRegistrations = services
            .Where(sd => sd.ServiceType.IsGenericType &&
                   sd.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>))
            .Where(sd => sd.ImplementationType?.AssemblyQualifiedName != null && sd.ImplementationType.AssemblyQualifiedName.StartsWith("ID.OAuth.Google"))
            .ToList();

        // Make sure we have at least one validator registration
        validatorRegistrations.Count.ShouldBeGreaterThan(0, "No validators were registered");

    }
    //------------------------------------//


}//Cls


