using ID.Application.AppAbs.Messaging;
using ID.Domain.Utility.Exceptions;
using ID.Msg.Twilio;
using ID.Msg.Twilio.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ID.Msg.Twilio.Tests.Setup;

public class IdMsgTwilioSetupExtensionsTests
{
    private const string _valid_twilio_id = "ACtest1234567890abcdef1234567890";
    private const string _valid_twilio_password = "test_auth_token_1234567890";
    private const string _valid_twilio_from_number = "+15551234567";

    //-------------------------------------//
    // Tests for AddIdMsgTwilio with string parameters
    //-------------------------------------//

    [Fact]
    public void AddIdMsgTwilio_With_Valid_Parameters_Should_Register_Services_And_Configure_Options()
    {
        // Arrange
        var services = new ServiceCollection();

        var options = new IdMsgTwilioOptions
        {
            TwilioId = _valid_twilio_id,
            TwilioPassword = _valid_twilio_password,
            TwilioFromNumber = _valid_twilio_from_number
        };

        // Act
        services.AddMyIdMessagingTwilio(options);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Verify services are registered
        var smsService = serviceProvider.GetService<IIdSmsService>();
        var whatsAppService = serviceProvider.GetService<IIdWhatsAppService>();
        
        Assert.NotNull(smsService);
        Assert.NotNull(whatsAppService);
        Assert.IsType<IdTwilioSmsService>(smsService);
        Assert.IsType<IdTwilioWhatsAppService>(whatsAppService);

        // Verify options are configured
        var twilioOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;
        Assert.Equal(_valid_twilio_id, twilioOptions.TwilioId);
        Assert.Equal(_valid_twilio_password, twilioOptions.TwilioPassword);
        Assert.Equal(_valid_twilio_from_number, twilioOptions.TwilioFromNumber);
    }

    //-------------------------------------//

    [Theory]
    [InlineData(null, _valid_twilio_password, _valid_twilio_from_number)]
    [InlineData("", _valid_twilio_password, _valid_twilio_from_number)]
    [InlineData("   ", _valid_twilio_password, _valid_twilio_from_number)]
    public void AddIdMsgTwilio_Should_Throw_Exception_When_TwilioId_Invalid(string invalidTwilioId, string password, string fromNumber)
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new IdMsgTwilioOptions
        {
            TwilioId = invalidTwilioId,
            TwilioPassword = password,
            TwilioFromNumber = fromNumber
        };

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.AddMyIdMessagingTwilio(options));
        
        Assert.Contains("TwilioId", exception.Message);
    }

    //-------------------------------------//
    // Tests for AddIdMsgTwilio with IConfiguration
    //-------------------------------------//

    [Fact]
    public void AddIdMsgTwilio_With_Configuration_Should_Register_Services_And_Configure_Options()
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
        services.AddMyIdMessagingTwilio(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Verify services are registered
        var smsService = serviceProvider.GetService<IIdSmsService>();
        var whatsAppService = serviceProvider.GetService<IIdWhatsAppService>();
        
        Assert.NotNull(smsService);
        Assert.NotNull(whatsAppService);
        Assert.IsType<IdTwilioSmsService>(smsService);
        Assert.IsType<IdTwilioWhatsAppService>(whatsAppService);

        // Verify options are configured
        var twilioOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;
        Assert.Equal(_valid_twilio_id, twilioOptions.TwilioId);
        Assert.Equal(_valid_twilio_password, twilioOptions.TwilioPassword);
        Assert.Equal(_valid_twilio_from_number, twilioOptions.TwilioFromNumber);
    }

    //-------------------------------------//

    [Fact]
    public void AddIdMsgTwilio_With_Configuration_Section_Should_Register_Services_And_Configure_Options()
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
        services.AddMyIdMessagingTwilio(configuration, "Twilio");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Verify services are registered
        var smsService = serviceProvider.GetService<IIdSmsService>();
        var whatsAppService = serviceProvider.GetService<IIdWhatsAppService>();
        
        Assert.NotNull(smsService);
        Assert.NotNull(whatsAppService);

        // Verify options are configured
        var twilioOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;
        Assert.Equal(_valid_twilio_id, twilioOptions.TwilioId);
        Assert.Equal(_valid_twilio_password, twilioOptions.TwilioPassword);
        Assert.Equal(_valid_twilio_from_number, twilioOptions.TwilioFromNumber);
    }

    //-------------------------------------//

    [Fact]
    public void AddIdMsgTwilio_Should_Throw_Exception_When_Configuration_Is_Null()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            services.AddMyIdMessagingTwilio((IConfiguration)null!));
        
        Assert.Equal("configuration", exception.ParamName);
    }

    //-------------------------------------//
    // Tests for AddIdMsgTwilio with Action<IdMsgTwilioOptions> (legacy compatibility)
    //-------------------------------------//

    [Fact]
    public void AddIdMsgTwilio_With_Action_Should_Register_Services_And_Configure_Options()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdMessagingTwilio(options =>
        {
            options.TwilioId = _valid_twilio_id;
            options.TwilioPassword = _valid_twilio_password;
            options.TwilioFromNumber = _valid_twilio_from_number;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Verify services are registered
        var smsService = serviceProvider.GetService<IIdSmsService>();
        var whatsAppService = serviceProvider.GetService<IIdWhatsAppService>();
        
        Assert.NotNull(smsService);
        Assert.NotNull(whatsAppService);
        Assert.IsType<IdTwilioSmsService>(smsService);
        Assert.IsType<IdTwilioWhatsAppService>(whatsAppService);

        // Verify options are configured
        var twilioOptions = serviceProvider.GetRequiredService<IOptions<IdMsgTwilioOptions>>().Value;
        Assert.Equal(_valid_twilio_id, twilioOptions.TwilioId);
        Assert.Equal(_valid_twilio_password, twilioOptions.TwilioPassword);
        Assert.Equal(_valid_twilio_from_number, twilioOptions.TwilioFromNumber);
    }

    //-------------------------------------//

    [Fact]
    public void AddIdMsgTwilio_With_Action_Should_Throw_Exception_When_Required_Properties_Missing()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<SetupDataException>(() => 
            services.AddMyIdMessagingTwilio(options =>
            {
                options.TwilioId = _valid_twilio_id;
                // Missing TwilioPassword and TwilioFromNumber
            }));
        
        Assert.Contains("TwilioPassword", exception.Message);
    }

    //-------------------------------------//

    [Fact]
    public void AddIdMsgTwilio_Services_Should_Be_Registered_As_Transient()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new IdMsgTwilioOptions
        {
            TwilioId = _valid_twilio_id,
            TwilioPassword = _valid_twilio_password,
            TwilioFromNumber = _valid_twilio_from_number
        };
        // Act
        services.AddMyIdMessagingTwilio(options);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        // Get services multiple times to verify they are different instances (transient)
        var smsService1 = serviceProvider.GetRequiredService<IIdSmsService>();
        var smsService2 = serviceProvider.GetRequiredService<IIdSmsService>();
        var whatsAppService1 = serviceProvider.GetRequiredService<IIdWhatsAppService>();
        var whatsAppService2 = serviceProvider.GetRequiredService<IIdWhatsAppService>();
        
        Assert.NotSame(smsService1, smsService2);
        Assert.NotSame(whatsAppService1, whatsAppService2);
    }

    //-------------------------------------//

    [Fact]
    public void AddIdMsgTwilio_Should_Replace_Existing_Service_Registrations()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Register a different implementation first
        services.AddTransient<IIdSmsService, MockSmsService>();
        services.AddTransient<IIdWhatsAppService, MockWhatsAppService>();

        // Act
        var options = new IdMsgTwilioOptions
        {
            TwilioId = _valid_twilio_id,
            TwilioPassword = _valid_twilio_password,
            TwilioFromNumber = _valid_twilio_from_number
        };
        // Act
        services.AddMyIdMessagingTwilio(options);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        var smsService = serviceProvider.GetRequiredService<IIdSmsService>();
        var whatsAppService = serviceProvider.GetRequiredService<IIdWhatsAppService>();
        
        // Should be the Twilio implementations, not the mock ones
        Assert.IsType<IdTwilioSmsService>(smsService);
        Assert.IsType<IdTwilioWhatsAppService>(whatsAppService);
    }

    //-------------------------------------//
    // Mock implementations for testing service replacement
    //-------------------------------------//

    private class MockSmsService : IIdSmsService
    {
        public Task<MyResults.BasicResult> SendMsgAsync(string number, string message)
        {
            return Task.FromResult(MyResults.BasicResult.Success());
        }
    }

    private class MockWhatsAppService : IIdWhatsAppService
    {
        public Task<MyResults.BasicResult> SendMsgAsync(string number, string message)
        {
            return Task.FromResult(MyResults.BasicResult.Success());
        }
    }
}
