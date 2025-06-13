using ID.Domain.Utility.Exceptions;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ID.Email.Base.Tests.Setup;

public class IdEmailBaseOptionsSetupTests
{
    [Fact]
    public void ConfigureEmailBaseOptions_WithValidOptions_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "Test App");
        
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            FromName = "Test Application",
            LogoUrl = "https://example.com/logo.png",
            ToAddresses = ["to1@example.com", "to2@example.com"],
            CcAddresses = ["cc@example.com"],
            BccAddresses = ["bcc1@example.com", "bcc2@example.com"],
            ColorHexBrand = "#123456"
        };

        // Act
        services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.FromAddress.ShouldBe("test@example.com");
        configuredOptions.FromName.ShouldBe("Test Application");
        configuredOptions.LogoUrl.ShouldBe("https://example.com/logo.png");
        configuredOptions.ToAddresses.ShouldBe(emailOptions.ToAddresses);
        configuredOptions.CcAddresses.ShouldBe(emailOptions.CcAddresses);
        configuredOptions.BccAddresses.ShouldBe(emailOptions.BccAddresses);
        configuredOptions.ColorHexBrand.ShouldBe("#123456");
    }

    //-------------------------------------//
    
    [Fact]
    public void ConfigureEmailBaseOptions_WithEmptyFromName_Should_Use_ApplicationName()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "My Application");
        
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            FromName = string.Empty // Empty from name
        };

        // Act
        services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.FromName.ShouldBe("My Application");
    }

    //-------------------------------------//
      [Fact]
    public void ConfigureEmailBaseOptions_WithNullFromName_Should_Use_ApplicationName()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "My Application");
        
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            FromName = null! // Null from name
        };

        // Act
        services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.FromName.ShouldBe("My Application");
    }

    //-------------------------------------//
        [Fact]
    public void ConfigureEmailBaseOptions_WithWhitespaceFromName_Should_Use_ApplicationName()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "My Application");
        
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            FromName = "   " // Whitespace from name
        };

        // Act
        services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.FromName.ShouldBe("My Application");
    }

    //-------------------------------------//

[Fact]
    public void ConfigureEmailBaseOptions_WithEmptyColorHexBrand_Should_Use_DefaultColor()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "Test App");
        
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            ColorHexBrand = string.Empty // Empty color
        };

        // Act
        services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.ColorHexBrand.ShouldBe(MyIdEmailDefaultValues.COLOR_HEX_BRAND);
    }

    //-------------------------------------//

[Fact]
    public void ConfigureEmailBaseOptions_WithNullColorHexBrand_Should_Use_DefaultColor()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "Test App");
        
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            ColorHexBrand = null! // Null color
        };

        // Act
        services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.ColorHexBrand.ShouldBe(MyIdEmailDefaultValues.COLOR_HEX_BRAND);
    }

    //-------------------------------------//

[Fact]
    public void ConfigureEmailBaseOptions_WithWhitespaceColorHexBrand_Should_Use_DefaultColor()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "Test App");
        
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            ColorHexBrand = "   " // Whitespace color
        };

        // Act
        services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.ColorHexBrand.ShouldBe(MyIdEmailDefaultValues.COLOR_HEX_BRAND);
    }

    //-------------------------------------//

[Fact]
    public void ConfigureEmailBaseOptions_WithNullFromAddress_Should_ThrowSetupDataException()
    {
        // Arrange
        var services = new ServiceCollection();
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = null! // Null from address - should throw
        };

        // Act & Assert
        Should.Throw<SetupDataException>(() => services.ConfigureEmailBaseOptions(emailOptions))
            .Message.ShouldContain(nameof(IdEmailBaseOptions.FromAddress));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailBaseOptions_WithEmptyFromAddress_Should_ThrowSetupDataException()
    {
        // Arrange
        var services = new ServiceCollection();
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = string.Empty // Empty from address - should throw
        };

        // Act & Assert
        Should.Throw<SetupDataException>(() => services.ConfigureEmailBaseOptions(emailOptions))
            .Message.ShouldContain(nameof(IdEmailBaseOptions.FromAddress));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailBaseOptions_WithWhitespaceFromAddress_Should_ThrowSetupDataException()
    {
        // Arrange
        var services = new ServiceCollection();
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "   " // Whitespace from address - should throw
        };

        // Act & Assert
        Should.Throw<SetupDataException>(() => services.ConfigureEmailBaseOptions(emailOptions))
            .Message.ShouldContain(nameof(IdEmailBaseOptions.FromAddress));
    }

    //-------------------------------------//

[Fact]
    public void ConfigureEmailBaseOptions_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "Test App");
        
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com"
        };

        // Act
        var result = services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        result.ShouldBeSameAs(services);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailBaseOptions_WithConfiguration_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FromAddress"] = "config@example.com",
                ["FromName"] = "Config Application",
                ["LogoUrl"] = "https://config.com/logo.png",
                ["ColorHexBrand"] = "#789ABC",
                ["ToAddresses:0"] = "to1@config.com",
                ["ToAddresses:1"] = "to2@config.com",
                ["CcAddresses:0"] = "cc@config.com",
                ["BccAddresses:0"] = "bcc1@config.com",
                ["BccAddresses:1"] = "bcc2@config.com"
            })
            .Build();

        // Act
        services.ConfigureEmailBaseOptions(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.FromAddress.ShouldBe("config@example.com");
        configuredOptions.FromName.ShouldBe("Config Application");
        configuredOptions.LogoUrl.ShouldBe("https://config.com/logo.png");
        configuredOptions.ColorHexBrand.ShouldBe("#789ABC");
        configuredOptions.ToAddresses.ShouldBe(["to1@config.com", "to2@config.com"]);
        configuredOptions.CcAddresses.ShouldBe(["cc@config.com"]);
        configuredOptions.BccAddresses.ShouldBe(["bcc1@config.com", "bcc2@config.com"]);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailBaseOptions_WithConfigurationSection_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Email:FromAddress"] = "section@example.com",
                ["Email:FromName"] = "Section Application",
                ["Email:LogoUrl"] = "https://section.com/logo.png",
                ["Email:ColorHexBrand"] = "#DEF456"
            })
            .Build();

        // Act
        services.ConfigureEmailBaseOptions(configuration, "Email");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.FromAddress.ShouldBe("section@example.com");
        configuredOptions.FromName.ShouldBe("Section Application");
        configuredOptions.LogoUrl.ShouldBe("https://section.com/logo.png");
        configuredOptions.ColorHexBrand.ShouldBe("#DEF456");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailBaseOptions_WithNullConfiguration_Should_ThrowSetupDataException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act & Assert
        Should.Throw<SetupDataException>(() => services.ConfigureEmailBaseOptions(configuration))
            .Message.ShouldContain(nameof(configuration));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailBaseOptions_WithConfiguration_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FromAddress"] = "test@example.com"
            })
            .Build();

        // Act
        var result = services.ConfigureEmailBaseOptions(configuration);

        // Assert
        result.ShouldBeSameAs(services);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailBaseOptions_WithEmptyStringSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FromAddress"] = "root@example.com",
                ["FromName"] = "Root Application"
            })
            .Build();

        // Act
        services.ConfigureEmailBaseOptions(configuration, string.Empty);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.FromAddress.ShouldBe("root@example.com");
        configuredOptions.FromName.ShouldBe("Root Application");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailBaseOptions_WithWhitespaceSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FromAddress"] = "whitespace@example.com",
                ["FromName"] = "Whitespace Application"
            })
            .Build();

        // Act
        services.ConfigureEmailBaseOptions(configuration, "   ");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.FromAddress.ShouldBe("whitespace@example.com");
        configuredOptions.FromName.ShouldBe("Whitespace Application");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailBaseOptions_WithNullSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FromAddress"] = "nullsection@example.com",
                ["FromName"] = "Null Section Application"
            })
            .Build();

        // Act
        services.ConfigureEmailBaseOptions(configuration, null);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.FromAddress.ShouldBe("nullsection@example.com");
        configuredOptions.FromName.ShouldBe("Null Section Application");
    }

    //-------------------------------------//
    
    [Theory]
    [InlineData("test1@example.com", "Test Name 1")]
    [InlineData("test2@example.com", "Test Name 2")]
    [InlineData("another@domain.org", "Another Application")]
    public void ConfigureEmailBaseOptions_Should_Handle_Various_Parameter_Combinations(string fromAddress, string fromName)
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "Test App");
        
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = fromAddress,
            FromName = fromName
        };

        // Act
        services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.FromAddress.ShouldBe(fromAddress);
        configuredOptions.FromName.ShouldBe(fromName);
    }

    //-------------------------------------//
    
    [Fact]
    public void ConfigureEmailBaseOptions_Should_Preserve_Empty_Collections()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "Test App");
        
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            ToAddresses = [],
            CcAddresses = [],
            BccAddresses = []
        };

        // Act
        services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.ToAddresses.ShouldBeEmpty();
        configuredOptions.CcAddresses.ShouldBeEmpty();
        configuredOptions.BccAddresses.ShouldBeEmpty();
    }

    //-------------------------------------//
    
    [Fact]
    public void ConfigureEmailBaseOptions_Should_Preserve_Collection_Order()
    {
        // Arrange
        var services = new ServiceCollection();
        RegisterGlobalOptions(services, "Test App");
        
        var toAddresses = new[] { "to3@example.com", "to1@example.com", "to2@example.com" };
        var ccAddresses = new[] { "cc2@example.com", "cc1@example.com" };
        var bccAddresses = new[] { "bcc1@example.com", "bcc3@example.com", "bcc2@example.com" };

        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            ToAddresses = toAddresses,
            CcAddresses = ccAddresses,
            BccAddresses = bccAddresses
        };

        // Act
        services.ConfigureEmailBaseOptions(emailOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailBaseOptions>>().Value;

        configuredOptions.ToAddresses.ShouldBe(toAddresses, ignoreOrder: false);
        configuredOptions.CcAddresses.ShouldBe(ccAddresses, ignoreOrder: false);
        configuredOptions.BccAddresses.ShouldBe(bccAddresses, ignoreOrder: false);
    }

    //-------------------------------------//


    private static void RegisterGlobalOptions(IServiceCollection services, string applicationName)
    {
        services.Configure<IdGlobalOptions>(options =>
        {
            options.ApplicationName = applicationName;
        });
    }

}//Cls
