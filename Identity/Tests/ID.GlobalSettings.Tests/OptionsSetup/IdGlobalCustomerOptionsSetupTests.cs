using System;
using System.Collections.Generic;
using ID.GlobalSettings.Exceptions;
using ID.GlobalSettings.Setup;
using ID.GlobalSettings.Setup.Defaults;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace ID.GlobalSettings.Tests.OptionsSetup;

public class IdGlobalCustomerOptionsSetupTests
{
    [Fact]
    public void ConfigureCustomerGlobalOptions_WithValidOptions_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "https://customer.example.com/accounts",
            MaxTeamPosition = 15,
            MinTeamPosition = 2,
            MaxTeamSize = 50
        };

        // Act
        services.ConfigureCustomerGlobalOptions(customerOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalSetupOptions_CUSTOMER>>().Value;

        configuredOptions.CustomerAccountsUrl.ShouldBe("https://customer.example.com/accounts");
        configuredOptions.MaxTeamPosition.ShouldBe(15);
        configuredOptions.MinTeamPosition.ShouldBe(2);
        configuredOptions.MaxTeamSize.ShouldBe(50);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptions_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "https://customer.example.com/accounts",
            MaxTeamPosition = 10,
            MinTeamPosition = 1,
            MaxTeamSize = 25
        };

        // Act
        var result = services.ConfigureCustomerGlobalOptions(customerOptions);

        // Assert
        result.ShouldBeSameAs(services);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptions_WithNullCustomerAccountsUrl_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = null!, // Invalid
            MaxTeamPosition = 10,
            MinTeamPosition = 1,
            MaxTeamSize = 25
        };

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() => services.ConfigureCustomerGlobalOptions(customerOptions))
            .Message.ShouldContain(nameof(IdGlobalSetupOptions_CUSTOMER.CustomerAccountsUrl));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptions_WithEmptyCustomerAccountsUrl_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = string.Empty, // Invalid
            MaxTeamPosition = 10,
            MinTeamPosition = 1,
            MaxTeamSize = 25
        };

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() => services.ConfigureCustomerGlobalOptions(customerOptions))
            .Message.ShouldContain(nameof(IdGlobalSetupOptions_CUSTOMER.CustomerAccountsUrl));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptions_WithWhitespaceCustomerAccountsUrl_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "   ", // Invalid
            MaxTeamPosition = 10,
            MinTeamPosition = 1,
            MaxTeamSize = 25
        };

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() => services.ConfigureCustomerGlobalOptions(customerOptions))
            .Message.ShouldContain(nameof(IdGlobalSetupOptions_CUSTOMER.CustomerAccountsUrl));
    }

    //-------------------------------------//

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ConfigureCustomerGlobalOptions_WithInvalidMaxTeamSize_Should_DefaultToValidValue(int invalidTeamSize)
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "https://customer.example.com/accounts",
            MaxTeamPosition = 10,
            MinTeamPosition = 1,
            MaxTeamSize = invalidTeamSize // Invalid - should be defaulted by setter
        };

        // Act - Should not throw exception because setter defaults invalid values
        services.ConfigureCustomerGlobalOptions(customerOptions);        // Assert - The value should be defaulted by the setter
        Assert.Equal(IdGlobalDefaultValues.Customer.MAX_TEAM_SIZE, customerOptions.MaxTeamSize);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ConfigureCustomerGlobalOptions_WithInvalidMinTeamPosition_Should_DefaultToValidValue(int invalidPosition)
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "https://customer.example.com/accounts",
            MaxTeamPosition = 10,
            MinTeamPosition = invalidPosition, // Invalid - should be defaulted by setter
            MaxTeamSize = 25
        };

        // Act - Should not throw exception because setter defaults invalid values
        services.ConfigureCustomerGlobalOptions(customerOptions);        // Assert - The value should be defaulted by the setter
        Assert.Equal(IdGlobalDefaultValues.Customer.MIN_TEAM_POSITION, customerOptions.MinTeamPosition);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ConfigureCustomerGlobalOptions_WithInvalidMaxTeamPosition_Should_DefaultToValidValue(int invalidPosition)
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "https://customer.example.com/accounts",
            MaxTeamPosition = invalidPosition, // Invalid - should be defaulted by setter
            MinTeamPosition = 1,
            MaxTeamSize = 25
        };

        // Act - Should not throw exception because setter defaults invalid values
        services.ConfigureCustomerGlobalOptions(customerOptions);        
        // Assert - The value should be defaulted by the setter
        Assert.Equal(IdGlobalDefaultValues.Customer.MAX_TEAM_POSITION, customerOptions.MaxTeamPosition);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptions_WithMinPositionGreaterThanMaxPosition_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "https://customer.example.com/accounts",
            MaxTeamPosition = 3,
            MinTeamPosition = 5, // Invalid: greater than max
            MaxTeamSize = 25
        };

        // Act & Assert
        Should.Throw<GlobalSettingInvalidSetupDataException>(() => services.ConfigureCustomerGlobalOptions(customerOptions))
            .Message.ShouldContain("must not be greater than");
    }

    //-------------------------------------//

    [Theory]
    [InlineData(1, 5, 10)]
    [InlineData(2, 10, 20)]
    [InlineData(1, 1, 1)]
    public void ConfigureCustomerGlobalOptions_WithValidValues_Should_Configure_Successfully(int minPosition, int maxPosition, int maxTeamSize)
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "https://customer.example.com/accounts",
            MaxTeamPosition = maxPosition,
            MinTeamPosition = minPosition,
            MaxTeamSize = maxTeamSize
        };

        // Act
        services.ConfigureCustomerGlobalOptions(customerOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalSetupOptions_CUSTOMER>>().Value;
        configuredOptions.MinTeamPosition.ShouldBe(minPosition);
        configuredOptions.MaxTeamPosition.ShouldBe(maxPosition);
        configuredOptions.MaxTeamSize.ShouldBe(maxTeamSize);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptionsFromConfiguration_WithValidConfiguration_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CustomerAccountsUrl"] = "https://config-customer.example.com/accounts",
                ["MaxTeamPosition"] = "12",
                ["MinTeamPosition"] = "3",
                ["MaxTeamSize"] = "40"
            })
            .Build();

        // Act
        services.ConfigureCustomerGlobalOptions(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalSetupOptions_CUSTOMER>>().Value;

        configuredOptions.CustomerAccountsUrl.ShouldBe("https://config-customer.example.com/accounts");
        configuredOptions.MaxTeamPosition.ShouldBe(12);
        configuredOptions.MinTeamPosition.ShouldBe(3);
        configuredOptions.MaxTeamSize.ShouldBe(40);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptionsFromConfiguration_WithSection_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CustomerSettings:CustomerAccountsUrl"] = "https://section-customer.example.com/accounts",
                ["CustomerSettings:MaxTeamPosition"] = "20",
                ["CustomerSettings:MinTeamPosition"] = "5",
                ["CustomerSettings:MaxTeamSize"] = "100"
            })
            .Build();

        // Act
        services.ConfigureCustomerGlobalOptions(configuration, "CustomerSettings");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalSetupOptions_CUSTOMER>>().Value;

        configuredOptions.CustomerAccountsUrl.ShouldBe("https://section-customer.example.com/accounts");
        configuredOptions.MaxTeamPosition.ShouldBe(20);
        configuredOptions.MinTeamPosition.ShouldBe(5);
        configuredOptions.MaxTeamSize.ShouldBe(100);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptionsFromConfiguration_WithNullConfiguration_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() =>
            services.ConfigureCustomerGlobalOptions(configuration))
            .Message.ShouldContain(nameof(configuration));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptionsFromConfiguration_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CustomerAccountsUrl"] = "https://customer.example.com/accounts"
            })
            .Build();

        // Act
        var result = services.ConfigureCustomerGlobalOptions(configuration);

        // Assert
        result.ShouldBeSameAs(services);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptionsFromConfiguration_WithEmptySection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CustomerAccountsUrl"] = "https://root-customer.example.com/accounts",
                ["MaxTeamSize"] = "30"
            })
            .Build();

        // Act
        services.ConfigureCustomerGlobalOptions(configuration, string.Empty);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalSetupOptions_CUSTOMER>>().Value;

        configuredOptions.CustomerAccountsUrl.ShouldBe("https://root-customer.example.com/accounts");
        configuredOptions.MaxTeamSize.ShouldBe(30);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptionsFromConfiguration_WithWhitespaceSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CustomerAccountsUrl"] = "https://whitespace-customer.example.com/accounts",
                ["MaxTeamSize"] = "35"
            })
            .Build();

        // Act
        services.ConfigureCustomerGlobalOptions(configuration, "   ");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalSetupOptions_CUSTOMER>>().Value;

        configuredOptions.CustomerAccountsUrl.ShouldBe("https://whitespace-customer.example.com/accounts");
        configuredOptions.MaxTeamSize.ShouldBe(35);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptionsFromConfiguration_WithNullSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CustomerAccountsUrl"] = "https://null-customer.example.com/accounts",
                ["MaxTeamSize"] = "45"
            })
            .Build();

        // Act
        services.ConfigureCustomerGlobalOptions(configuration, null);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalSetupOptions_CUSTOMER>>().Value;

        configuredOptions.CustomerAccountsUrl.ShouldBe("https://null-customer.example.com/accounts");
        configuredOptions.MaxTeamSize.ShouldBe(45);
    }

    //-------------------------------------//

    [Theory]
    [InlineData("https://customer1.com/accounts", 10)]
    [InlineData("https://customer2.domain.org/accounts", 25)]
    [InlineData("https://company-customer.com/portal", 50)]
    public void ConfigureCustomerGlobalOptions_Should_Handle_Various_Parameter_Combinations(string accountsUrl, int maxTeamSize)
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = accountsUrl,
            MaxTeamPosition = 10,
            MinTeamPosition = 1,
            MaxTeamSize = maxTeamSize
        };

        // Act
        services.ConfigureCustomerGlobalOptions(customerOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalSetupOptions_CUSTOMER>>().Value;

        configuredOptions.CustomerAccountsUrl.ShouldBe(accountsUrl);
        configuredOptions.MaxTeamSize.ShouldBe(maxTeamSize);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptions_WithMinimalValidOptions_Should_Configure_Successfully()
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "https://a.com",
            MaxTeamPosition = 1,
            MinTeamPosition = 1,
            MaxTeamSize = 1
        };

        // Act & Assert
        Should.NotThrow(() => services.ConfigureCustomerGlobalOptions(customerOptions));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureCustomerGlobalOptions_WithDefaultValues_Should_Use_Defaults_From_Properties()
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "https://customer.example.com/accounts"
            // Other properties will use their default values from the property setters
        };

        // Act
        services.ConfigureCustomerGlobalOptions(customerOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalSetupOptions_CUSTOMER>>().Value;

        configuredOptions.CustomerAccountsUrl.ShouldBe("https://customer.example.com/accounts");
        // Verify that defaults are applied (these should match the defaults in IdGlobalSetupOptions_CUSTOMER)
        configuredOptions.MaxTeamPosition.ShouldBeGreaterThan(0);
        configuredOptions.MinTeamPosition.ShouldBeGreaterThan(0);
        configuredOptions.MaxTeamSize.ShouldBeGreaterThan(0);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 10)]
    [InlineData(5, 5)]
    public void ConfigureCustomerGlobalOptions_WithEqualMinMaxPositions_Should_Configure_Successfully(int minPosition, int maxPosition)
    {
        // Arrange
        var services = new ServiceCollection();
        var customerOptions = new IdGlobalSetupOptions_CUSTOMER
        {
            CustomerAccountsUrl = "https://customer.example.com/accounts",
            MaxTeamPosition = maxPosition,
            MinTeamPosition = minPosition,
            MaxTeamSize = 25
        };

        // Act & Assert
        Should.NotThrow(() => services.ConfigureCustomerGlobalOptions(customerOptions));

        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalSetupOptions_CUSTOMER>>().Value;
        configuredOptions.MinTeamPosition.ShouldBe(minPosition);
        configuredOptions.MaxTeamPosition.ShouldBe(maxPosition);
    }

    //-------------------------------------//

}//Cls
