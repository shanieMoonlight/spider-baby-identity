using FluentValidation;
using ID.Application.Customers.Abstractions;
using ID.Application.Customers.Setup;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace ID.Application.Customers.Tests.Setup;
public class IdApplicationSetupExtensionsTests
{
    private readonly IdGlobalSetupOptions_CUSTOMER _setupOptions = new()
    {
        CustomerAccountsUrl = "https://example.com/accounts"
    };

    //------------------------------------//

    [Fact]
    public void AddIdApplication_ShouldRegisterValidators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdCustomers(_setupOptions);

        // Assert

        // Check if any validators are registered from the Customers assembly
        var validatorRegistrations = services
            .Where(sd => sd.ServiceType.IsGenericType &&
                   sd.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>))
            .Where(sd => sd.ImplementationType?.AssemblyQualifiedName != null && sd.ImplementationType.AssemblyQualifiedName.StartsWith("ID.Application.Customers"))
            .ToList();

        // Make sure we have at least one validator registration
        validatorRegistrations.Count.ShouldBeGreaterThan(0, "No validators were registered");

    }

    //------------------------------------//

    [Fact]
    public void AddIdApplication_ShouldIIdCustomerRegistrationService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdCustomers(_setupOptions);

        // Assert

        // Check if any validators are registered from the Customers assembly
        var validatorRegistrations = services
            .Where(sd => sd.ServiceType.Name == nameof(IIdCustomerRegistrationService))
            .ToList();

        validatorRegistrations.Count.ShouldBe(1, "No IIdCustomerRegistrationService registered");

    }


}//Cls





//=============================================================================//



internal record ServiceIdx(ServiceDescriptor Service, int Idx)
{
    public Type? ImplementationType => Service.ImplementationType;

}


//=============================================================================//

