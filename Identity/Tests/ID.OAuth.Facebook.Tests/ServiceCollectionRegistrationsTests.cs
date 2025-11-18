using FluentValidation;
using ID.Application.Customers.Setup;
using ID.Domain.Entities.AppUsers;
using ID.OAuth.Facebook.HttpService.Abs;
using ID.OAuth.Facebook.Services.Abs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace ID.OAuth.Facebook.Tests;

public class ServiceCollectionRegistrationsTests
{
    [Fact]
    public void AddMyIdFacebookOAuth_ActionRegistersExpectedServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdFacebookOAuth(opts =>
        {
            opts.AppId = "app123";
            opts.AppSecret = "secret";
            opts.GraphApiBaseUrl = "https://graph.facebook.com";
        });

        // Assert - ensure core services are registered
        services.Any(sd => sd.ServiceType == typeof(IFacebookHttpClient)).ShouldBeTrue();
        services.Any(sd => sd.ServiceType == typeof(IFacebookAuthenticationService)).ShouldBeTrue();
        services.Any(sd => sd.ServiceType == typeof(IFacebookClientUtilities)).ShouldBeTrue();
        services.Any(sd => sd.ServiceType == typeof(IFindOrCreateService<AppUser>)).ShouldBeTrue();
    }

    //----------------------//

    [Fact]
    public void AddMyIdFacebookOAuth_ConfigurationRegistersExpectedServices()
    {
        // Arrange
        var inMemory = new Dictionary<string, string?>
        {
            ["FacebookOAuth:AppId"] = "appcfg",
            ["FacebookOAuth:AppSecret"] = "secretcfg",
            ["FacebookOAuth:GraphApiBaseUrl"] = "https://graph.facebook.com"
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();
        var services = new ServiceCollection();

        // Act
        services.AddMyIdFacebookOAuth(config, "FacebookOAuth");

        // Assert
        services.Any(sd => sd.ServiceType == typeof(IFacebookHttpClient)).ShouldBeTrue();
        services.Any(sd => sd.ServiceType == typeof(IFacebookAuthenticationService)).ShouldBeTrue();
        services.Any(sd => sd.ServiceType == typeof(IFacebookClientUtilities)).ShouldBeTrue();
        services.Any(sd => sd.ServiceType == typeof(IFindOrCreateService<AppUser>)).ShouldBeTrue();
    }

    //----------------------//



    [Fact]
    public void AddIdApplication_ShouldRegisterValidators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdFacebookOAuth(opts =>
        {
            opts.AppId = "app123";
            opts.AppSecret = "secret";
            opts.GraphApiBaseUrl = "https://graph.facebook.com";
        });

        // Assert

        // Check if any validators are registered from the Customers assembly
        var validatorRegistrations = services
            .Where(sd => sd.ServiceType.IsGenericType &&
                   sd.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>))
            .Where(sd => sd.ImplementationType?.AssemblyQualifiedName != null && sd.ImplementationType.AssemblyQualifiedName.StartsWith("ID.OAuth.Facebook"))
            .ToList();

        // Make sure we have at least one validator registration
        validatorRegistrations.Count.ShouldBeGreaterThan(0, "No validators were registered");

    }

    //----------------------//

    [Fact]
    public void AddMyIdFacebookOAuth_RegistersJsonSerializerOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdFacebookOAuth(opts =>
        {
            opts.AppId = "app123";
            opts.AppSecret = "secret";
            opts.GraphApiBaseUrl = "https://graph.facebook.com";
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
