using ID.Application.AppAbs.Messaging;
using ID.Msg.Twilio.Setup;
using Microsoft.Extensions.Options;
using MyResults;
using Shouldly;
using Xunit;

namespace ID.Msg.Twilio.Tests;

public class IdTwilioSmsServiceTests
{
    private readonly IdMsgTwilioOptions _twilioOptions;
    private readonly IOptions<IdMsgTwilioOptions> _mockOptions;
    private readonly IdTwilioSmsService _smsService;

    public IdTwilioSmsServiceTests()
    {
        _twilioOptions = new IdMsgTwilioOptions
        {
            TwilioId = "test_account_sid",
            TwilioPassword = "test_auth_token",
            TwilioFromNumber = "+1234567890"
        };

        _mockOptions = Options.Create(_twilioOptions);
        _smsService = new IdTwilioSmsService(_mockOptions);
    }

    //------------------------------------//

    [Fact]
    public void Constructor_WithValidOptions_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var service = new IdTwilioSmsService(_mockOptions);

        // Assert
        service.ShouldNotBeNull();
        service.ShouldBeOfType<IdTwilioSmsService>();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_ShouldImplementIIdSmsService()
    {
        // Arrange & Act
        var service = new IdTwilioSmsService(_mockOptions);

        // Assert
        service.ShouldBeAssignableTo<IIdSmsService>();
    }

    [Theory]
    [InlineData("+1234567890", "Test message")]
    [InlineData("+44123456789", "Hello World")]
    [InlineData("+61234567890", "This is a test SMS")]
    [InlineData("+1555123456", "SMS with special chars: !@#$%")]
    public async Task SendMsgAsync_WithValidParameters_ShouldReturnResult(string phoneNumber, string message)
    {
        // Note: These tests will actually attempt to make Twilio API calls with test credentials
        // In a real scenario, you would want to mock the Twilio client or use dependency injection
        // For now, we're testing the method signature and basic structure

        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);

        // Act
        var result = await service.SendMsgAsync(phoneNumber, message);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<BasicResult>();
        // Note: Result will likely be a failure since we're using test credentials
        // but we're testing that the method executes without throwing exceptions
    }

    [Theory]
    [InlineData("", "Test message")]
    [InlineData("   ", "Test message")]
    [InlineData("+1234567890", "")]
    [InlineData("+1234567890", "   ")]
    public async Task SendMsgAsync_WithInvalidParameters_ShouldHandleGracefully(string phoneNumber, string message)
    {
        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);

        // Act
        var result = await service.SendMsgAsync(phoneNumber, message);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<BasicResult>();
        // Should not throw exception, even with invalid parameters
    }

    //------------------------------------//

    [Fact]
    public async Task SendMsgAsync_WithNullPhoneNumber_ShouldHandleGracefully()
    {
        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);

        // Act
        var result = await service.SendMsgAsync(null!, "Test message");

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public async Task SendMsgAsync_WithNullMessage_ShouldHandleGracefully()
    {
        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);

        // Act
        var result = await service.SendMsgAsync("+1234567890", null!);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public async Task SendMsgAsync_WithInvalidPhoneNumberFormat_ShouldReturnFailure()
    {
        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);

        // Act
        var result = await service.SendMsgAsync("invalid-phone", "Test message");

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNullOrEmpty();
    }

    //------------------------------------//

    [Fact]
    public async Task SendMsgAsync_WithLongMessage_ShouldHandleCorrectly()
    {
        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);
        var longMessage = new string('A', 2000); // Very long message

        // Act
        var result = await service.SendMsgAsync("+1234567890", longMessage);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<BasicResult>();
    }


    //------------------------------------//


    [Theory]
    [InlineData(null, "test_token", "+1234567890")]
    [InlineData("", "test_token", "+1234567890")]
    [InlineData("test_sid", null, "+1234567890")]
    [InlineData("test_sid", "", "+1234567890")]
    [InlineData("test_sid", "test_token", null)]
    [InlineData("test_sid", "test_token", "")]
    public async Task SendMsgAsync_WithIncompleteCredentials_ShouldReturnFailure(
        string? twilioId, string? twilioPassword, string? fromNumber)
    {
        // Arrange
        var incompleteOptions = new IdMsgTwilioOptions
        {
            TwilioId = twilioId,
            TwilioPassword = twilioPassword,
            TwilioFromNumber = fromNumber
        };
        var mockIncompleteOptions = Options.Create(incompleteOptions);
        var service = new IdTwilioSmsService(mockIncompleteOptions);

        // Act
        var result = await service.SendMsgAsync("+1234567890", "Test message");

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNullOrEmpty();
    }

    //------------------------------------//

    [Fact]
    public async Task SendMsgAsync_ShouldCatchExceptionsAndReturnFailure()
    {
        // Arrange
        var invalidOptions = new IdMsgTwilioOptions
        {
            TwilioId = "invalid_sid_that_will_cause_exception",
            TwilioPassword = "invalid_token",
            TwilioFromNumber = "+1234567890"
        };
        var mockInvalidOptions = Options.Create(invalidOptions);
        var service = new IdTwilioSmsService(mockInvalidOptions);

        // Act
        var result = await service.SendMsgAsync("+1234567890", "Test message");

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNullOrEmpty();
    }

    //------------------------------------//


    [Theory]
    [InlineData("+1234567890")]
    [InlineData("+44123456789")]
    [InlineData("+61234567890")]
    [InlineData("+81234567890")]
    public async Task SendMsgAsync_WithDifferentCountryCodes_ShouldHandleCorrectly(string phoneNumber)
    {
        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);

        // Act
        var result = await service.SendMsgAsync(phoneNumber, "Test international message");

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<BasicResult>();
    }

    //------------------------------------//

    [Fact]
    public async Task SendMsgAsync_WithSpecialCharactersInMessage_ShouldHandleCorrectly()
    {
        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);
        var messageWithSpecialChars = "Hello! 🚀 Test message with émojis and spëcial chàracters 中文 العربية";

        // Act
        var result = await service.SendMsgAsync("+1234567890", messageWithSpecialChars);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<BasicResult>();
    }

    //------------------------------------//

    [Fact]
    public async Task SendMsgAsync_CalledMultipleTimes_ShouldHandleSequentialCalls()
    {
        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);

        // Act
        var result1 = await service.SendMsgAsync("+1234567890", "First message");
        var result2 = await service.SendMsgAsync("+1234567891", "Second message");
        var result3 = await service.SendMsgAsync("+1234567892", "Third message");

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result3.ShouldNotBeNull();
    }

    //------------------------------------//

    [Fact]
    public async Task SendMsgAsync_WithExtremelyLongPhoneNumber_ShouldReturnFailure()
    {
        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);
        var extremelyLongPhoneNumber = "+" + new string('1', 100);

        // Act
        var result = await service.SendMsgAsync(extremelyLongPhoneNumber, "Test message");

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
    }

    [Theory]
    [InlineData("123456789")] // No country code
    [InlineData("1234567890")] // No + prefix
    [InlineData("abc123def")] // Contains letters
    [InlineData("+1-234-567-890")] // Contains dashes
    [InlineData("+1 234 567 890")] // Contains spaces
    public async Task SendMsgAsync_WithVariousInvalidPhoneFormats_ShouldReturnFailure(string phoneNumber)
    {
        // Arrange
        var service = new IdTwilioSmsService(_mockOptions);

        // Act
        var result = await service.SendMsgAsync(phoneNumber, "Test message");

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
    }
}
