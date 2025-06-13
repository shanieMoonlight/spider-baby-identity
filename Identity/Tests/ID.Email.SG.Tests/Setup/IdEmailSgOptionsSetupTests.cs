using ID.Domain.Utility.Exceptions;
using ID.Email.SG.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace ID.Email.SG.Tests.Setup;

public class IdEmailSgOptionsSetupTests
{
    [Fact]
    public void ConfigureSendGridOptions_WithValidApiKey_Should_Configure_ApiKey_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var sgOptions = new IdEmailSgOptions
        {
            ApiKey = "SG.test-api-key-12345"
        };

        // Act
        services.ConfigureSendGridOptions(sgOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSgOptions>>().Value;

        configuredOptions.ApiKey.ShouldBe("SG.test-api-key-12345");
    }   
    
     //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_WithNullApiKey_Should_ThrowSetupDataException()
    {
        // Arrange
        var services = new ServiceCollection();
        var sgOptions = new IdEmailSgOptions
        {
            ApiKey = null // Null API key - should throw
        };

        // Act & Assert
        Should.Throw<SetupDataException>(() => services.ConfigureSendGridOptions(sgOptions))
            .Message.ShouldContain(nameof(IdEmailSgOptions.ApiKey));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_WithEmptyApiKey_Should_ThrowSetupDataException()
    {
        // Arrange
        var services = new ServiceCollection();
        var sgOptions = new IdEmailSgOptions
        {
            ApiKey = string.Empty // Empty API key - should throw
        };

        // Act & Assert
        Should.Throw<SetupDataException>(() => services.ConfigureSendGridOptions(sgOptions))
            .Message.ShouldContain(nameof(IdEmailSgOptions.ApiKey));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_WithWhitespaceApiKey_Should_ThrowSetupDataException()
    {
        // Arrange
        var services = new ServiceCollection();
        var sgOptions = new IdEmailSgOptions
        {
            ApiKey = "   " // Whitespace API key - should throw
        };

        // Act & Assert
        Should.Throw<SetupDataException>(() => services.ConfigureSendGridOptions(sgOptions))
            .Message.ShouldContain(nameof(IdEmailSgOptions.ApiKey));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var sgOptions = new IdEmailSgOptions
        {
            ApiKey = "SG.test-api-key"
        };

        // Act
        var result = services.ConfigureSendGridOptions(sgOptions);

        // Assert
        result.ShouldBeSameAs(services);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_WithConfiguration_Should_Configure_ApiKey_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiKey"] = "SG.config-api-key-67890"
            })
            .Build();

        // Act
        services.ConfigureSendGridOptions(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSgOptions>>().Value;

        configuredOptions.ApiKey.ShouldBe("SG.config-api-key-67890");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_WithConfigurationSection_Should_Configure_ApiKey_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SendGrid:ApiKey"] = "SG.section-api-key-abc123"
            })
            .Build();

        // Act
        services.ConfigureSendGridOptions(configuration, "SendGrid");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSgOptions>>().Value;

        configuredOptions.ApiKey.ShouldBe("SG.section-api-key-abc123");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_WithNullConfiguration_Should_ThrowSetupDataException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act & Assert
        Should.Throw<SetupDataException>(() => services.ConfigureSendGridOptions(configuration))
            .Message.ShouldContain(nameof(configuration));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_WithConfiguration_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiKey"] = "SG.test-api-key"
            })
            .Build();

        // Act
        var result = services.ConfigureSendGridOptions(configuration);

        // Assert
        result.ShouldBeSameAs(services);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_WithEmptyStringSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiKey"] = "SG.root-api-key"
            })
            .Build();

        // Act
        services.ConfigureSendGridOptions(configuration, string.Empty);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSgOptions>>().Value;

        configuredOptions.ApiKey.ShouldBe("SG.root-api-key");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_WithNullSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiKey"] = "SG.nullsection-api-key"
            })
            .Build();

        // Act
        services.ConfigureSendGridOptions(configuration, null);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSgOptions>>().Value;

        configuredOptions.ApiKey.ShouldBe("SG.nullsection-api-key");
    }

    //-------------------------------------//

    [Theory]
    [InlineData("SG.api-key-1")]
    [InlineData("SG.api-key-2")]
    [InlineData("SG.another-key-with-long-value-12345")]
    public void ConfigureSendGridOptions_Should_Handle_Various_ApiKey_Values(string apiKey)
    {
        // Arrange
        var services = new ServiceCollection();
        var sgOptions = new IdEmailSgOptions
        {
            ApiKey = apiKey
        };

        // Act
        services.ConfigureSendGridOptions(sgOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSgOptions>>().Value;

        configuredOptions.ApiKey.ShouldBe(apiKey);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureSendGridOptions_WithConfigurationMissingApiKey_Should_Allow_Binding()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act - Configuration binding doesn't validate at setup time
        services.ConfigureSendGridOptions(configuration);

        // Assert - The options should be configured even with missing ApiKey
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSgOptions>>().Value;

        configuredOptions.ApiKey.ShouldBeNull(); // This will be null since not provided in config
    }

    //-------------------------------------//

}//Cls
