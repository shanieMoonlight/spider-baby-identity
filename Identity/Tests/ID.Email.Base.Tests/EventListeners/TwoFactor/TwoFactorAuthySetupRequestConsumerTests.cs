using ID.Email.Base.Abs;
using ID.Email.Base.AppAbs;
using ID.Email.Base.AppImps;
using ID.Email.Base.EventListeners.TwoFactor;
using Id.Tests.Utility.Exceptions;
using ID.IntegrationEvents.Events.Account.TwoFactor;
using Microsoft.Extensions.Logging;
using Moq;
using MyResults;
using Xunit;
using ID.GlobalSettings.Errors;

namespace ID.Email.Base.Tests.EventListeners.TwoFactor;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class TwoFactorAuthySetupRequestConsumerTests
{
    private readonly Mock<IEmailDetailsTemplateGenerator> _mockTemplateGenerator;
    private readonly Mock<IIdEmailService> _mockEmailService;
    private readonly Mock<ILogger<TwoFactorAuthySetupRequestConsumer>> _mockLogger;
    private readonly TwoFactorAuthySetupRequestConsumer _consumer;
    private readonly Mock<IEmailDetails> _mockEmailDetails;

    public TwoFactorAuthySetupRequestConsumerTests()
    {
        _mockTemplateGenerator = new Mock<IEmailDetailsTemplateGenerator>();
        _mockEmailService = new Mock<IIdEmailService>();
        _mockLogger = new Mock<ILogger<TwoFactorAuthySetupRequestConsumer>>();
        _mockEmailDetails = new Mock<IEmailDetails>();
        
        _consumer = new TwoFactorAuthySetupRequestConsumer(
            _mockTemplateGenerator.Object,
            _mockEmailService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleEventAsync_WithValidEvent_ShouldGenerateTemplateAndSendEmail()
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            QrSrc = "data:image/png;base64,iVBORw0KGgoAAAANS...",
            ManualQrCode = "ABCD1234EFGH5678"
        };

        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            "Test User",
            "test@example.com",
            "data:image/png;base64,iVBORw0KGgoAAAANS...",
            "ABCD1234EFGH5678",
            "Authy",
            "Two-Factor Setup"), Times.Once);

        _mockEmailService.Verify(x => x.SendEmailAsync(_mockEmailDetails.Object), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithEmptyName_ShouldStillGenerateTemplate()
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "",
            QrSrc = "qr-source",
            ManualQrCode = "1234567890"
        };

        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            "",
            "test@example.com",
            "qr-source",
            "1234567890",
            "Authy",
            "Two-Factor Setup"), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithNullName_ShouldStillGenerateTemplate()
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = null!,
            QrSrc = "qr-source",
            ManualQrCode = "1234567890"
        };

        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            null,
            "test@example.com",
            "qr-source",
            "1234567890",
            "Authy",
            "Two-Factor Setup"), Times.Once);
    }

    [Theory]
    [InlineData("John Doe", "john@example.com", "qr1", "CODE123")]
    [InlineData("Jane Smith", "jane@test.org", "qr2", "XYZ789")]
    [InlineData("User With Spaces", "user@domain.net", "qr3", "ABC!@#456")]
    [InlineData("", "empty@name.com", "qr4", "EMPTY789")]
    public async Task HandleEventAsync_WithVariousInputs_ShouldPassCorrectParameters(
        string name, string email, string qrSrc, string manualQrCode)
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = email,
            Name = name,
            QrSrc = qrSrc,
            ManualQrCode = manualQrCode
        };

        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            name,
            email,
            qrSrc,
            manualQrCode,
            "Authy",
            "Two-Factor Setup"), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WhenTemplateGenerationFails_ShouldLogException()
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            QrSrc = "qr-source",
            ManualQrCode = "1234567890"
        };

        var expectedException = new Exception("Template generation failed");
        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_mockLogger, IdErrorEvents.Email.TwoFactor, expectedException);
        _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()), Times.Never);
    }

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceFails_ShouldLogFailure()
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            QrSrc = "qr-source",
            ManualQrCode = "1234567890"
        };

        var failureResult = BasicResult.Failure("Email sending failed");
        
        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(failureResult);

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        ExceptionUtils.VerifyBasicResultLogging(_mockLogger, IdErrorEvents.Email.TwoFactor, failureResult);
    }

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceThrowsException_ShouldLogException()
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            QrSrc = "qr-source",
            ManualQrCode = "1234567890"
        };

        var expectedException = new Exception("Email service exception");
        
        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ThrowsAsync(expectedException);

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_mockLogger, IdErrorEvents.Email.TwoFactor, expectedException);
    }

    [Fact]
    public async Task HandleEventAsync_WithSpecialCharactersInQrCode_ShouldHandleCorrectly()
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            QrSrc = "data:image/png;base64,ABC123+/=",
            ManualQrCode = "ABC!@#$%^&*()123"
        };

        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            "Test User",
            "test@example.com",
            "data:image/png;base64,ABC123+/=",
            "ABC!@#$%^&*()123",
            "Authy",
            "Two-Factor Setup"), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithLongQrCode_ShouldHandleCorrectly()
    {
        // Arrange
        var longQrCode = new string('A', 500); // Very long QR code
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            QrSrc = "qr-source",
            ManualQrCode = longQrCode
        };

        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            "Test User",
            "test@example.com",
            "qr-source",
            longQrCode,
            "Authy",
            "Two-Factor Setup"), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_AlwaysUsesAuthyProvider_RegardlessOfInput()
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            QrSrc = "qr-source",
            ManualQrCode = "1234567890"
        };

        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            "Authy", // Should always be "Authy"
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_MultipleSequentialCalls_ShouldHandleEachCorrectly()
    {
        // Arrange
        var event1 = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "user1@example.com",
            Name = "User One",
            QrSrc = "qr1",
            ManualQrCode = "CODE1"
        };

        var event2 = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "user2@example.com",
            Name = "User Two",
            QrSrc = "qr2",
            ManualQrCode = "CODE2"
        };

        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(event1);
        await _consumer.HandleEventAsync(event2);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            "User One",
            "user1@example.com",
            "qr1",
            "CODE1",
            "Authy",
            "Two-Factor Setup"), Times.Once);

        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            "User Two",
            "user2@example.com",
            "qr2",
            "CODE2",
            "Authy",
            "Two-Factor Setup"), Times.Once);

        _mockEmailService.Verify(x => x.SendEmailAsync(_mockEmailDetails.Object), Times.Exactly(2));
    }

    [Fact]
    public async Task HandleEventAsync_WithEmptyQrSrc_ShouldStillProceed()
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            QrSrc = "",
            ManualQrCode = "1234567890"
        };

        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            "Test User",
            "test@example.com",
            "",
            "1234567890",
            "Authy",
            "Two-Factor Setup"), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithEmptyManualQrCode_ShouldStillProceed()
    {
        // Arrange
        var integrationEvent = new TwoFactorAuthySetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Name = "Test User",
            QrSrc = "qr-source",
            ManualQrCode = ""
        };

        _mockTemplateGenerator
            .Setup(x => x.GenerateTwoFactorAuthTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(_mockEmailDetails.Object);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(integrationEvent);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateTwoFactorAuthTemplateAsync(
            "Test User",
            "test@example.com",
            "qr-source",
            "",
            "Authy",
            "Two-Factor Setup"), Times.Once);
    }
}
