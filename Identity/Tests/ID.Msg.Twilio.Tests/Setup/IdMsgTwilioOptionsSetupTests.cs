using Shouldly;
using ID.Domain.Utility.Exceptions;
using ID.Msg.Twilio.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ID.Msg.Twilio.Tests.Setup;

public class IdMsgTwilioOptionsSetupTests
{
    private const string _valid_twilio_id = "ACtest1234567890abcdef1234567890";
    private const string _valid_twilio_password = "test_auth_token_1234567890";
    private const string _valid_twilio_from_number = "+15551234567";

    //-------------------------------------//
    // Tests for ConfigureTwilioOptions with IdMsgTwilioOptions parameter
    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_With_Valid_Options_Should_Configure_Options()
    {
        // Arrange
        var services = new ServiceCollection();
        var twilioOptions = new IdMsgTwilioOptions
        {
            TwilioId = _valid_twilio_id,
            TwilioPassword = _valid_twilio_password,
            TwilioFromNumber = _valid_twilio_from_number
        };

        // Act
        services.ConfigureTwilioOptions(twilioOptions);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var configuredOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;

        Assert.Equal(_valid_twilio_id, configuredOptions.TwilioId);
        Assert.Equal(_valid_twilio_password, configuredOptions.TwilioPassword);
        Assert.Equal(_valid_twilio_from_number, configuredOptions.TwilioFromNumber);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_Should_Throw_Exception_When_Options_Null()
    {
        // Arrange
        var services = new ServiceCollection();
        IdMsgTwilioOptions? twilioOptions = null;

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.ConfigureTwilioOptions(twilioOptions!));

        exception.Message.ShouldContain("twilioOptions", Case.Insensitive);


    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_Should_Throw_Exception_When_TwilioId_Null()
    {
        // Arrange
        var services = new ServiceCollection();
        var twilioOptions = new IdMsgTwilioOptions
        {
            TwilioId = null,
            TwilioPassword = _valid_twilio_password,
            TwilioFromNumber = _valid_twilio_from_number
        };

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.ConfigureTwilioOptions(twilioOptions));
        
        Assert.Contains("TwilioId", exception.Message);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_Should_Throw_Exception_When_TwilioId_Empty()
    {
        // Arrange
        var services = new ServiceCollection();
        var twilioOptions = new IdMsgTwilioOptions
        {
            TwilioId = "",
            TwilioPassword = _valid_twilio_password,
            TwilioFromNumber = _valid_twilio_from_number
        };

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.ConfigureTwilioOptions(twilioOptions));
        
        Assert.Contains("TwilioId", exception.Message);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_Should_Throw_Exception_When_TwilioId_Whitespace()
    {
        // Arrange
        var services = new ServiceCollection();
        var twilioOptions = new IdMsgTwilioOptions
        {
            TwilioId = "   ",
            TwilioPassword = _valid_twilio_password,
            TwilioFromNumber = _valid_twilio_from_number
        };

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.ConfigureTwilioOptions(twilioOptions));
        
        Assert.Contains("TwilioId", exception.Message);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_Should_Throw_Exception_When_TwilioPassword_Null()
    {
        // Arrange
        var services = new ServiceCollection();
        var twilioOptions = new IdMsgTwilioOptions
        {
            TwilioId = _valid_twilio_id,
            TwilioPassword = null,
            TwilioFromNumber = _valid_twilio_from_number
        };

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.ConfigureTwilioOptions(twilioOptions));
        
        Assert.Contains("TwilioPassword", exception.Message);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_Should_Throw_Exception_When_TwilioPassword_Empty()
    {
        // Arrange
        var services = new ServiceCollection();
        var twilioOptions = new IdMsgTwilioOptions
        {
            TwilioId = _valid_twilio_id,
            TwilioPassword = "",
            TwilioFromNumber = _valid_twilio_from_number
        };

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.ConfigureTwilioOptions(twilioOptions));
        
        Assert.Contains("TwilioPassword", exception.Message);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_Should_Throw_Exception_When_TwilioFromNumber_Null()
    {
        // Arrange
        var services = new ServiceCollection();
        var twilioOptions = new IdMsgTwilioOptions
        {
            TwilioId = _valid_twilio_id,
            TwilioPassword = _valid_twilio_password,
            TwilioFromNumber = null
        };

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.ConfigureTwilioOptions(twilioOptions));
        
        Assert.Contains("TwilioFromNumber", exception.Message);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(_valid_twilio_id, null, _valid_twilio_from_number)]
    [InlineData(_valid_twilio_id, "", _valid_twilio_from_number)]
    [InlineData(_valid_twilio_id, "   ", _valid_twilio_from_number)]
    public void ConfigureTwilioOptions_Should_Throw_Exception_When_TwilioPassword_Invalid(string twilioId, string invalidPassword, string fromNumber)
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new IdMsgTwilioOptions
        {
            TwilioId = twilioId,
            TwilioPassword = invalidPassword,
            TwilioFromNumber = fromNumber
        };

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.ConfigureTwilioOptions(options));
        
        Assert.Contains("TwilioPassword", exception.Message);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(_valid_twilio_id, _valid_twilio_password, null)]
    [InlineData(_valid_twilio_id, _valid_twilio_password, "")]
    [InlineData(_valid_twilio_id, _valid_twilio_password, "   ")]
    public void ConfigureTwilioOptions_Should_Throw_Exception_When_TwilioFromNumber_Invalid(string twilioId, string password, string invalidFromNumber)
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new IdMsgTwilioOptions
        {
            TwilioId = twilioId,
            TwilioPassword = password,
            TwilioFromNumber = invalidFromNumber
        };

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.ConfigureTwilioOptions(options));
        
        Assert.Contains("TwilioFromNumber", exception.Message);
        Assert.Contains(nameof(IdMsgTwilioOptionsSetup), exception.Message);
    }

    //-------------------------------------//
    // Tests for ConfigureTwilioOptions with IConfiguration
    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_With_Configuration_Should_Configure_Options()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string>
        {
            ["TwilioId"] = _valid_twilio_id,
            ["TwilioPassword"] = _valid_twilio_password,
            ["TwilioFromNumber"] = _valid_twilio_from_number
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        // Act
        services.ConfigureTwilioOptions(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var twilioOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;

        Assert.Equal(_valid_twilio_id, twilioOptions.TwilioId);
        Assert.Equal(_valid_twilio_password, twilioOptions.TwilioPassword);
        Assert.Equal(_valid_twilio_from_number, twilioOptions.TwilioFromNumber);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_With_Configuration_Section_Should_Configure_Options()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string>
        {
            ["Twilio:TwilioId"] = _valid_twilio_id,
            ["Twilio:TwilioPassword"] = _valid_twilio_password,
            ["Twilio:TwilioFromNumber"] = _valid_twilio_from_number
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        // Act
        services.ConfigureTwilioOptions(configuration, "Twilio");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var twilioOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;

        Assert.Equal(_valid_twilio_id, twilioOptions.TwilioId);
        Assert.Equal(_valid_twilio_password, twilioOptions.TwilioPassword);
        Assert.Equal(_valid_twilio_from_number, twilioOptions.TwilioFromNumber);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_Should_Throw_Exception_When_Configuration_Is_Null()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            services.ConfigureTwilioOptions((IConfiguration)null!));
        
        Assert.Equal("configuration", exception.ParamName);
        Assert.Contains("Configuration cannot be null when setting up Twilio options.", exception.Message);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_With_Empty_Configuration_Should_Configure_Options_With_Null_Values()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.ConfigureTwilioOptions(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var twilioOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;

        Assert.Null(twilioOptions.TwilioId);
        Assert.Null(twilioOptions.TwilioPassword);
        Assert.Null(twilioOptions.TwilioFromNumber);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_With_Partial_Configuration_Should_Configure_Available_Options()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string>
        {
            ["TwilioId"] = _valid_twilio_id,
            // Missing TwilioPassword and TwilioFromNumber
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        // Act
        services.ConfigureTwilioOptions(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var twilioOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;

        Assert.Equal(_valid_twilio_id, twilioOptions.TwilioId);
        Assert.Null(twilioOptions.TwilioPassword);
        Assert.Null(twilioOptions.TwilioFromNumber);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_With_Nested_Section_Should_Configure_Options()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string>
        {
            ["Messaging:Providers:Twilio:TwilioId"] = _valid_twilio_id,
            ["Messaging:Providers:Twilio:TwilioPassword"] = _valid_twilio_password,
            ["Messaging:Providers:Twilio:TwilioFromNumber"] = _valid_twilio_from_number
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        // Act
        services.ConfigureTwilioOptions(configuration, "Messaging:Providers:Twilio");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var twilioOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;

        Assert.Equal(_valid_twilio_id, twilioOptions.TwilioId);
        Assert.Equal(_valid_twilio_password, twilioOptions.TwilioPassword);
        Assert.Equal(_valid_twilio_from_number, twilioOptions.TwilioFromNumber);
    }

    //-------------------------------------//

    [Fact]
    public void ConfigureTwilioOptions_With_WhiteSpace_SectionName_Should_Use_Root_Configuration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string>
        {
            ["TwilioId"] = _valid_twilio_id,
            ["TwilioPassword"] = _valid_twilio_password,
            ["TwilioFromNumber"] = _valid_twilio_from_number
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        // Act
        services.ConfigureTwilioOptions(configuration, "   ");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var twilioOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;

        Assert.Equal(_valid_twilio_id, twilioOptions.TwilioId);
        Assert.Equal(_valid_twilio_password, twilioOptions.TwilioPassword);
        Assert.Equal(_valid_twilio_from_number, twilioOptions.TwilioFromNumber);
    }
}
