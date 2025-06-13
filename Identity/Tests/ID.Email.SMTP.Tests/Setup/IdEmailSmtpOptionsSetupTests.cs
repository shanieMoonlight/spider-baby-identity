namespace ID.Email.SMTP.Tests.Setup;

public class IdEmailSmtpOptionsSetupTests
{
    [Fact]
    public void ConfigureEmailSmtpOptions_WithValidOptions_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            // Base options
            FromAddress = "smtp@example.com",
            FromName = "SMTP Test",
            LogoUrl = "https://smtp.example.com/logo.png",
            ColorHexBrand = "#123456",

            // SMTP-specific options
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "smtp_user@example.com",
            SmtpPassword = "smtp_password123"
        };

        // Act
        services.ConfigureEmailSmtpOptions(smtpOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSmtpOptions>>().Value;

        configuredOptions.SmtpServerAddress.ShouldBe("smtp.example.com");
        configuredOptions.SmtpPortNumber.ShouldBe(587);
        configuredOptions.SmtpUsernameOrEmail.ShouldBe("smtp_user@example.com");
        configuredOptions.SmtpPassword.ShouldBe("smtp_password123");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptions_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        // Act
        var result = services.ConfigureEmailSmtpOptions(smtpOptions);

        // Assert
        result.ShouldBeSameAs(services);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptions_WithNullSmtpServerAddress_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = null, // Invalid
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        // Act & Assert
        Should.Throw<IdEmailSmtpMissingSetupException>(() => services.ConfigureEmailSmtpOptions(smtpOptions))
            .Message.ShouldContain(nameof(IdEmailSmtpOptions.SmtpServerAddress));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptions_WithEmptySmtpServerAddress_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = string.Empty, // Invalid
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        // Act & Assert
        Should.Throw<IdEmailSmtpMissingSetupException>(() => services.ConfigureEmailSmtpOptions(smtpOptions))
            .Message.ShouldContain(nameof(IdEmailSmtpOptions.SmtpServerAddress));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptions_WithWhitespaceSmtpServerAddress_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "   ", // Invalid
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        // Act & Assert
        Should.Throw<IdEmailSmtpMissingSetupException>(() => services.ConfigureEmailSmtpOptions(smtpOptions))
            .Message.ShouldContain(nameof(IdEmailSmtpOptions.SmtpServerAddress));
    }

    //-------------------------------------//

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-587)]
    public void ConfigureEmailSmtpOptions_WithInvalidPortNumber_Should_ThrowException(int invalidPort)
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = invalidPort, // Invalid
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        // Act & Assert
        Should.Throw<IdEmailSmtpInvalidSetupException>(() => services.ConfigureEmailSmtpOptions(smtpOptions))
            .Message.ShouldContain("SMTP port number must be greater than 0");
    }

    //-------------------------------------//

    [Theory]
    [InlineData(25)]
    [InlineData(465)]
    [InlineData(587)]
    [InlineData(2525)]
    public void ConfigureEmailSmtpOptions_WithValidPortNumbers_Should_Configure_Successfully(int validPort)
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = validPort,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        // Act
        services.ConfigureEmailSmtpOptions(smtpOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSmtpOptions>>().Value;
        configuredOptions.SmtpPortNumber.ShouldBe(validPort);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptions_WithNullUsername_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = null, // Invalid
            SmtpPassword = "password123"
        };

        // Act & Assert
        Should.Throw<IdEmailSmtpMissingSetupException>(() => services.ConfigureEmailSmtpOptions(smtpOptions))
            .Message.ShouldContain(nameof(IdEmailSmtpOptions.SmtpUsernameOrEmail));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptions_WithEmptyUsername_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = string.Empty, // Invalid
            SmtpPassword = "password123"
        };

        // Act & Assert
        Should.Throw<IdEmailSmtpMissingSetupException>(() => services.ConfigureEmailSmtpOptions(smtpOptions))
            .Message.ShouldContain(nameof(IdEmailSmtpOptions.SmtpUsernameOrEmail));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptions_WithWhitespaceUsername_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "   ", // Invalid
            SmtpPassword = "password123"
        };

        // Act & Assert
        Should.Throw<IdEmailSmtpMissingSetupException>(() => services.ConfigureEmailSmtpOptions(smtpOptions))
            .Message.ShouldContain(nameof(IdEmailSmtpOptions.SmtpUsernameOrEmail));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptions_WithNullPassword_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = null // Invalid
        };

        // Act & Assert
        Should.Throw<IdEmailSmtpMissingSetupException>(() => services.ConfigureEmailSmtpOptions(smtpOptions))
            .Message.ShouldContain(nameof(IdEmailSmtpOptions.SmtpPassword));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptions_WithEmptyPassword_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = string.Empty // Invalid
        };

        // Act & Assert
        Should.Throw<IdEmailSmtpMissingSetupException>(() => services.ConfigureEmailSmtpOptions(smtpOptions))
            .Message.ShouldContain(nameof(IdEmailSmtpOptions.SmtpPassword));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptions_WithWhitespacePassword_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "   " // Invalid
        };

        // Act & Assert
        Should.Throw<IdEmailSmtpMissingSetupException>(() => services.ConfigureEmailSmtpOptions(smtpOptions))
            .Message.ShouldContain(nameof(IdEmailSmtpOptions.SmtpPassword));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptionsFromConfiguration_WithValidConfiguration_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SmtpServerAddress"] = "config.smtp.example.com",
                ["SmtpPortNumber"] = "465",
                ["SmtpUsernameOrEmail"] = "config_user@example.com",
                ["SmtpPassword"] = "config_password123",
                ["FromAddress"] = "config@example.com",
                ["FromName"] = "Config SMTP"
            })
            .Build();

        // Act
        services.ConfigureEmailSmtpOptionsFromConfiguration(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSmtpOptions>>().Value;

        configuredOptions.SmtpServerAddress.ShouldBe("config.smtp.example.com");
        configuredOptions.SmtpPortNumber.ShouldBe(465);
        configuredOptions.SmtpUsernameOrEmail.ShouldBe("config_user@example.com");
        configuredOptions.SmtpPassword.ShouldBe("config_password123");
        configuredOptions.FromAddress.ShouldBe("config@example.com");
        configuredOptions.FromName.ShouldBe("Config SMTP");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptionsFromConfiguration_WithSection_Should_Configure_Options_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Smtp:SmtpServerAddress"] = "section.smtp.example.com",
                ["Smtp:SmtpPortNumber"] = "2525",
                ["Smtp:SmtpUsernameOrEmail"] = "section_user@example.com",
                ["Smtp:SmtpPassword"] = "section_password123",
                ["Smtp:FromAddress"] = "section@example.com"
            })
            .Build();

        // Act
        services.ConfigureEmailSmtpOptionsFromConfiguration(configuration, "Smtp");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSmtpOptions>>().Value;

        configuredOptions.SmtpServerAddress.ShouldBe("section.smtp.example.com");
        configuredOptions.SmtpPortNumber.ShouldBe(2525);
        configuredOptions.SmtpUsernameOrEmail.ShouldBe("section_user@example.com");
        configuredOptions.SmtpPassword.ShouldBe("section_password123");
        configuredOptions.FromAddress.ShouldBe("section@example.com");
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptionsFromConfiguration_WithNullConfiguration_Should_ThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act & Assert
        Should.Throw<IdEmailSmtpMissingSetupException>(() =>
            services.ConfigureEmailSmtpOptionsFromConfiguration(configuration))
            .Message.ShouldContain(nameof(configuration));
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptionsFromConfiguration_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SmtpServerAddress"] = "smtp.example.com"
            })
            .Build();

        // Act
        var result = services.ConfigureEmailSmtpOptionsFromConfiguration(configuration);

        // Assert
        result.ShouldBeSameAs(services);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptionsFromConfiguration_WithEmptySection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SmtpServerAddress"] = "root.smtp.example.com",
                ["SmtpPortNumber"] = "587"
            })
            .Build();

        // Act
        services.ConfigureEmailSmtpOptionsFromConfiguration(configuration, string.Empty);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSmtpOptions>>().Value;

        configuredOptions.SmtpServerAddress.ShouldBe("root.smtp.example.com");
        configuredOptions.SmtpPortNumber.ShouldBe(587);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptionsFromConfiguration_WithWhitespaceSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SmtpServerAddress"] = "whitespace.smtp.example.com",
                ["SmtpPortNumber"] = "25"
            })
            .Build();

        // Act
        services.ConfigureEmailSmtpOptionsFromConfiguration(configuration, "   ");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSmtpOptions>>().Value;

        configuredOptions.SmtpServerAddress.ShouldBe("whitespace.smtp.example.com");
        configuredOptions.SmtpPortNumber.ShouldBe(25);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureEmailSmtpOptionsFromConfiguration_WithNullSection_Should_Use_RootConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SmtpServerAddress"] = "null.smtp.example.com",
                ["SmtpPortNumber"] = "465"
            })
            .Build();

        // Act
        services.ConfigureEmailSmtpOptionsFromConfiguration(configuration, null);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSmtpOptions>>().Value;

        configuredOptions.SmtpServerAddress.ShouldBe("null.smtp.example.com");
        configuredOptions.SmtpPortNumber.ShouldBe(465);
    }

    //-------------------------------------//

    [Theory]
    [InlineData("smtp1.example.com", "user1@example.com")]
    [InlineData("smtp2.domain.org", "user2@domain.org")]
    [InlineData("mail.company.com", "admin@company.com")]
    public void ConfigureEmailSmtpOptions_Should_Handle_Various_Parameter_Combinations(string serverAddress, string username)
    {
        // Arrange
        var services = new ServiceCollection();
        var smtpOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = serverAddress,
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = username,
            SmtpPassword = "password123"
        };

        // Act
        services.ConfigureEmailSmtpOptions(smtpOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdEmailSmtpOptions>>().Value;

        configuredOptions.SmtpServerAddress.ShouldBe(serverAddress);
        configuredOptions.SmtpUsernameOrEmail.ShouldBe(username);
    }

    //-------------------------------------//

    [Fact]
    public void ValidateOptions_WithValidOptions_Should_NotThrow()
    {
        // Arrange
        var validOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        // Act & Assert
        Should.NotThrow(() => IdEmailSmtpOptionsSetup.ValidateOptions(validOptions));
    }

    //-------------------------------------//

    [Fact]
    public void ValidateOptions_WithMinimalValidOptions_Should_NotThrow()
    {
        // Arrange
        var minimalOptions = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "s",
            SmtpPortNumber = 1,
            SmtpUsernameOrEmail = "u",
            SmtpPassword = "p"
        };

        // Act & Assert
        Should.NotThrow(() => IdEmailSmtpOptionsSetup.ValidateOptions(minimalOptions));
    }

    //-------------------------------------//

    [Fact]
    public void ValidateOptions_Is_Public_And_Accessible()
    {
        // This test ensures the ValidateOptions method is publicly accessible
        // which is useful for external validation scenarios

        // Arrange
        var options = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        // Act & Assert
        Should.NotThrow(() => IdEmailSmtpOptionsSetup.ValidateOptions(options));
    }

    //-------------------------------------//

}//Cls
