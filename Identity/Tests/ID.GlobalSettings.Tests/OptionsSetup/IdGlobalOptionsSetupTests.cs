using ID.GlobalSettings.Exceptions;
using ID.GlobalSettings.Setup;
using ID.GlobalSettings.Setup.Defaults;
using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;

namespace ID.GlobalSettings.Tests.OptionsSetup;

public class IdGlobalOptionsSetupTests
{
    [Fact]
    public void ConfigureGlobalOptions_WithValidOptions_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
          applicationName: "Test App",
          mntcAccountsUrl: "https://example.com/mntc",
          defaultMaxTeamPosition: 10,
          defaultMinTeamPosition: 1,
          claimTypePrefix: "TestClaim",
          refreshTokensEnabled: true,
          phoneTokenTimeSpan: TimeSpan.FromMinutes(15)
      );

        // Act
        services.ConfigureGlobalOptions(globalOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalOptions>>().Value;        configuredOptions.ApplicationName.ShouldBe("Test App");
        configuredOptions.MntcAccountsUrl.ShouldBe("https://example.com/mntc");
        configuredOptions.MntcTeamMaxPosition.ShouldBe(10);
        configuredOptions.MntcTeamMinPosition.ShouldBe(1);
        configuredOptions.PhoneTokenTimeSpan.ShouldBe(TimeSpan.FromMinutes(15));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act
        var result = services.ConfigureGlobalOptions(globalOptions);

        // Assert
        result.ShouldBeSameAs(services);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_WithNullApplicationName_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();

        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
           applicationName: null!, // Invalid
           mntcAccountsUrl: "https://example.com/mntc",
           defaultMaxTeamPosition: 5,
           defaultMinTeamPosition: 1,
           claimTypePrefix: "TestClaim",
           refreshTokensEnabled: false,
           phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
       );

        globalOptions.ApplicationName = null!; // Ensure it's null for the test

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() => services.ConfigureGlobalOptions(globalOptions))
            .Message.ShouldContain(nameof(IdGlobalOptions.ApplicationName));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_WithEmptyApplicationName_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection(); var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: string.Empty, // Invalid
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() => services.ConfigureGlobalOptions(globalOptions))
            .Message.ShouldContain(nameof(IdGlobalOptions.ApplicationName));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_WithWhitespaceApplicationName_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection(); var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "   ", // Invalid
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() => services.ConfigureGlobalOptions(globalOptions))
            .Message.ShouldContain(nameof(IdGlobalOptions.ApplicationName));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_WithNullMntcAccountsUrl_Should_ThrowException()
    {

        // Arrange
        var services = new ServiceCollection();
        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: null!, // Invalid
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        globalOptions.MntcAccountsUrl = null!; // Ensure it's null for the test

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() => services.ConfigureGlobalOptions(globalOptions))
            .Message.ShouldContain(nameof(IdGlobalOptions.MntcAccountsUrl));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_WithEmptyMntcAccountsUrl_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection(); var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: string.Empty, // Invalid
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() => services.ConfigureGlobalOptions(globalOptions))
            .Message.ShouldContain(nameof(IdGlobalOptions.MntcAccountsUrl));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_WithWhitespaceMntcAccountsUrl_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection(); var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "   ", // Invalid
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() => services.ConfigureGlobalOptions(globalOptions))
            .Message.ShouldContain(nameof(IdGlobalOptions.MntcAccountsUrl));
    }

    //-------------------------------------//

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ConfigureGlobalOptions_WithInvalidDefaultMaxTeamPosition_Should_DefaultToValidValue(int invalidPosition)
    {
        // Arrange
        var services = new ServiceCollection(); var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: invalidPosition, // Invalid - should be defaulted by setter
            defaultMinTeamPosition: 1,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act - Should not throw exception because setter defaults invalid values
        services.ConfigureGlobalOptions(globalOptions);

        // Assert - The value should be defaulted by the setter
        Assert.Equal(IdGlobalDefaultValues.MAX_TEAM_POSITION, globalOptions.MntcTeamMaxPosition);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ConfigureGlobalOptions_WithInvalidDefaultMinTeamPosition_Should_DefaultToValidValue(int invalidPosition)
    {
        // Arrange
        var services = new ServiceCollection();

        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: invalidPosition, // Invalid - should be defaulted by setter
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act - Should not throw exception because setter defaults invalid values
        services.ConfigureGlobalOptions(globalOptions);

        // Assert - The value should be defaulted by the setter
        Assert.Equal(IdGlobalDefaultValues.MIN_TEAM_POSITION, globalOptions.MntcTeamMinPosition);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_WithMinPositionGreaterThanMaxPosition_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection(); var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: 3,
            defaultMinTeamPosition: 5, // Invalid: greater than max
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act & Assert
        Should.Throw<GlobalSettingInvalidSetupDataException>(() => services.ConfigureGlobalOptions(globalOptions))
            .Message.ShouldContain("must not be greater than");
    }    //-------------------------------------//

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void ConfigureGlobalOptions_WithInvalidPhoneTokenTimeSpan_Should_DefaultToValidValue(int minutes)
    {
        // Arrange
        var services = new ServiceCollection(); var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(minutes) // Invalid - should be defaulted by setter
        );

        // Act - Should not throw exception because setter defaults invalid values
        services.ConfigureGlobalOptions(globalOptions);

        // Assert - The value should be defaulted by the setter
        Assert.Equal(IdGlobalDefaultValues.PHONE_TOKEN_EXPIRE_TIME_SPAN, globalOptions.PhoneTokenTimeSpan);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 10)]
    [InlineData(1, 1)]
    public void ConfigureGlobalOptions_WithValidTeamPositions_Should_Configure_Successfully(int minPosition, int maxPosition)
    {
        // Arrange
        var services = new ServiceCollection(); var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: maxPosition,
            defaultMinTeamPosition: minPosition,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act
        services.ConfigureGlobalOptions(globalOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalOptions>>().Value;
        configuredOptions.MntcTeamMinPosition.ShouldBe(minPosition);
        configuredOptions.MntcTeamMaxPosition.ShouldBe(maxPosition);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptionsFromConfiguration_WithValidConfiguration_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApplicationName"] = "Config Test App",
                ["MntcAccountsUrl"] = "https://config.example.com/mntc",
                ["MntcTeamMaxPosition"] = "8",
                ["MntcTeamMinPosition"] = "2",
                ["ClaimTypePrefix"] = "ConfigClaim",
                ["RefreshTokensEnabled"] = "true",
                ["PhoneTokenTimeSpan"] = "00:10:00"
            })
            .Build();

        // Act
        services.ConfigureGlobalOptions(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalOptions>>().Value;

        configuredOptions.ApplicationName.ShouldBe("Config Test App");
        configuredOptions.MntcAccountsUrl.ShouldBe("https://config.example.com/mntc");
        configuredOptions.MntcTeamMaxPosition.ShouldBe(8);
        configuredOptions.MntcTeamMinPosition.ShouldBe(2);
        configuredOptions.PhoneTokenTimeSpan.ShouldBe(TimeSpan.FromMinutes(10));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptionsFromConfiguration_WithSection_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["GlobalSettings:ApplicationName"] = "Section Test App",
                ["GlobalSettings:MntcAccountsUrl"] = "https://section.example.com/mntc",
                ["GlobalSettings:MntcTeamMaxPosition"] = "12",
                ["GlobalSettings:MntcTeamMinPosition"] = "3",
                ["GlobalSettings:PhoneTokenTimeSpan"] = "00:20:00"
            })
            .Build();

        // Act
        services.ConfigureGlobalOptions(configuration, "GlobalSettings");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalOptions>>().Value;

        configuredOptions.ApplicationName.ShouldBe("Section Test App");
        configuredOptions.MntcAccountsUrl.ShouldBe("https://section.example.com/mntc");
        configuredOptions.MntcTeamMaxPosition.ShouldBe(12);
        configuredOptions.MntcTeamMinPosition.ShouldBe(3);
        configuredOptions.PhoneTokenTimeSpan.ShouldBe(TimeSpan.FromMinutes(20));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptionsFromConfiguration_WithNullConfiguration_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act & Assert
        Should.Throw<GlobalSettingMissingSetupDataException>(() =>
            services.ConfigureGlobalOptions(configuration))
            .Message.ShouldContain(nameof(configuration));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptionsFromConfiguration_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApplicationName"] = "Test App"
            })
            .Build();

        // Act
        var result = services.ConfigureGlobalOptions(configuration);

        // Assert
        result.ShouldBeSameAs(services);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptionsFromConfiguration_WithEmptySection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApplicationName"] = "Root Config App",
                ["MntcAccountsUrl"] = "https://root.example.com/mntc"
            })
            .Build();

        // Act
        services.ConfigureGlobalOptions(configuration, string.Empty);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalOptions>>().Value;

        configuredOptions.ApplicationName.ShouldBe("Root Config App");
        configuredOptions.MntcAccountsUrl.ShouldBe("https://root.example.com/mntc");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptionsFromConfiguration_WithWhitespaceSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApplicationName"] = "Whitespace Config App",
                ["MntcAccountsUrl"] = "https://whitespace.example.com/mntc"
            })
            .Build();

        // Act
        services.ConfigureGlobalOptions(configuration, "   ");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalOptions>>().Value;

        configuredOptions.ApplicationName.ShouldBe("Whitespace Config App");
        configuredOptions.MntcAccountsUrl.ShouldBe("https://whitespace.example.com/mntc");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptionsFromConfiguration_WithNullSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApplicationName"] = "Null Section App",
                ["MntcAccountsUrl"] = "https://null.example.com/mntc"
            })
            .Build();

        // Act
        services.ConfigureGlobalOptions(configuration, null);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalOptions>>().Value;

        configuredOptions.ApplicationName.ShouldBe("Null Section App");
        configuredOptions.MntcAccountsUrl.ShouldBe("https://null.example.com/mntc");
    }

    //-------------------------------------//

    [Theory]
    [InlineData("App1", "https://app1.com/mntc")]
    [InlineData("App2", "https://app2.domain.org/mntc")]
    [InlineData("Company App", "https://company.com/accounts")]
    public void ConfigureGlobalOptions_Should_Handle_Various_Parameter_Combinations(string appName, string accountsUrl)
    {
        // Arrange
        var services = new ServiceCollection(); var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: appName,
            mntcAccountsUrl: accountsUrl,
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act
        services.ConfigureGlobalOptions(globalOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalOptions>>().Value;

        configuredOptions.ApplicationName.ShouldBe(appName);
        configuredOptions.MntcAccountsUrl.ShouldBe(accountsUrl);
    }    //-------------------------------------//

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ConfigureGlobalOptions_WithInvalidSuperTeamMaxPosition_Should_DefaultToValidValue(int invalidPosition)
    {
        // Arrange
        var services = new ServiceCollection();

        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            superTeamMaxPosition: invalidPosition, // Invalid - should be defaulted by setter
            superTeamMinPosition: 1,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act - Should not throw exception because setter defaults invalid values
        services.ConfigureGlobalOptions(globalOptions);

        // Assert - The value should be defaulted by the setter
        Assert.Equal(IdGlobalDefaultValues.MAX_TEAM_POSITION, globalOptions.SuperTeamMaxPosition);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ConfigureGlobalOptions_WithInvalidSuperTeamMinPosition_Should_DefaultToValidValue(int invalidPosition)
    {
        // Arrange
        var services = new ServiceCollection();

        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            superTeamMaxPosition: 5,
            superTeamMinPosition: invalidPosition, // Invalid - should be defaulted by setter
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act - Should not throw exception because setter defaults invalid values
        services.ConfigureGlobalOptions(globalOptions);

        // Assert - The value should be defaulted by the setter
        Assert.Equal(IdGlobalDefaultValues.MIN_TEAM_POSITION, globalOptions.SuperTeamMinPosition);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_WithSuperTeamMinPositionGreaterThanMaxPosition_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();

        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            superTeamMaxPosition: 3,
            superTeamMinPosition: 5, // Invalid: greater than max
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(5)
        );

        // Act & Assert
        Should.Throw<GlobalSettingInvalidSetupDataException>(() => services.ConfigureGlobalOptions(globalOptions))
            .Message.ShouldContain("SuperTeamMinPosition must not be greater than SuperTeamMaxPosition");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_WithValidSuperTeamPositions_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();

        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test App",
            mntcAccountsUrl: "https://example.com/mntc",
            defaultMaxTeamPosition: 5,
            defaultMinTeamPosition: 1,
            superTeamMaxPosition: 8,
            superTeamMinPosition: 2,
            claimTypePrefix: "TestClaim",
            refreshTokensEnabled: true,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(15)
        );

        // Act
        services.ConfigureGlobalOptions(globalOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdGlobalOptions>>().Value;

        configuredOptions.SuperTeamMaxPosition.ShouldBe(8);
        configuredOptions.SuperTeamMinPosition.ShouldBe(2);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureGlobalOptions_WithMinimalValidOptions_Should_Configure_Successfully()
    {
        // Arrange
        var services = new ServiceCollection(); var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "A",
            mntcAccountsUrl: "https://a.com",
            defaultMaxTeamPosition: 1,
            defaultMinTeamPosition: 1,
            claimTypePrefix: "C",
            refreshTokensEnabled: false,
            phoneTokenTimeSpan: TimeSpan.FromMilliseconds(1)
        );

        // Act & Assert
        Should.NotThrow(() => services.ConfigureGlobalOptions(globalOptions));
    }

    //-------------------------------------//

}//Cls
