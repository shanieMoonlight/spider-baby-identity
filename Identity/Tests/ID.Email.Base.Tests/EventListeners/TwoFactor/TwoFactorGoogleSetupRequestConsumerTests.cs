using ID.Email.Base.Abs;
using ID.Email.Base.EventListeners.TwoFactor;
using Id.Tests.Utility.Exceptions;
using ID.IntegrationEvents.Events.Account.TwoFactor;
using Microsoft.Extensions.Logging;
using MyResults;
using ID.GlobalSettings.Errors;

namespace ID.Email.Base.Tests.EventListeners.TwoFactor;

public class TwoFactorGoogleSetupRequestConsumerTests
{
    private readonly Mock<ILogger<TwoFactorGoogleSetupRequestConsumer>> _loggerMock;
    private readonly Mock<IEmailDetailsTemplateGenerator> _templateGeneratorMock;
    private readonly Mock<IIdEmailService> _emailServiceMock;
    private readonly Mock<IEmailDetails> _emailDetailsMock;
    private readonly TwoFactorGoogleSetupRequestConsumer _consumer;

    public TwoFactorGoogleSetupRequestConsumerTests()
    {
        _loggerMock = new Mock<ILogger<TwoFactorGoogleSetupRequestConsumer>>();
        _templateGeneratorMock = new Mock<IEmailDetailsTemplateGenerator>();
        _emailServiceMock = new Mock<IIdEmailService>();
        _emailDetailsMock = new Mock<IEmailDetails>();

        _consumer = new TwoFactorGoogleSetupRequestConsumer(
            _templateGeneratorMock.Object,
            _emailServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleEventAsync_WithValidEvent_ShouldGenerateTemplateAndSendEmail()
    {
        // Arrange
        var googleSetupEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            QrSrc = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==",
            Name = "Test User",
            ManualQrCode = "JBSWY3DPEHPK3PXP"
        };

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(googleSetupEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            "Test User",
            "test@example.com",
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==",
            "JBSWY3DPEHPK3PXP",
            "Two-Factor Setup"), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(_emailDetailsMock.Object), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithDifferentQrData_ShouldUseCorrectParameters()
    {
        // Arrange
        var googleSetupEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "jane@example.com",
            QrSrc = "data:image/png;base64,DIFFERENTQRDATA",
            Name = "Jane Doe",
            ManualQrCode = "ABCDEFGHIJKLMNOP"
        };

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(googleSetupEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            "Jane Doe",
            "jane@example.com",
            "data:image/png;base64,DIFFERENTQRDATA",
            "ABCDEFGHIJKLMNOP",
            "Two-Factor Setup"), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WhenTemplateGenerationFails_ShouldLogException()
    {
        // Arrange
        var googleSetupEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            QrSrc = "data:image/png;base64,testdata",
            Name = "Test User",
            ManualQrCode = "TESTCODE123"
        };

        var exception = new Exception("Template generation failed");

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act
        await _consumer.HandleEventAsync(googleSetupEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()), Times.Never);

        ExceptionUtils.VerifyExceptionLogging(_loggerMock, IdErrorEvents.Email.TwoFactor, exception);
    }

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceReturnsFailure_ShouldLogError()
    {
        // Arrange
        var googleSetupEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            QrSrc = "data:image/png;base64,testdata",
            Name = "Test User",
            ManualQrCode = "TESTCODE123"
        };

        var errorMessage = "Email service failed";
        var failureResult = BasicResult.Failure(errorMessage);

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(failureResult);

        // Act
        await _consumer.HandleEventAsync(googleSetupEvent);

        // Assert
        ExceptionUtils.VerifyBasicResultLogging(_loggerMock, IdErrorEvents.Email.TwoFactor, failureResult);
    }

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceThrowsException_ShouldLogException()
    {
        // Arrange
        var googleSetupEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            QrSrc = "data:image/png;base64,testdata",
            Name = "Test User",
            ManualQrCode = "TESTCODE123"
        };

        var exception = new Exception("Email service exception");

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ThrowsAsync(exception);

        // Act
        await _consumer.HandleEventAsync(googleSetupEvent);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_loggerMock, IdErrorEvents.Email.TwoFactor, exception);
    }

    [Theory]
    [InlineData("test@example.com", "Test User", "data:image/png;base64,ABC123", "SECRETKEY123")]
    [InlineData("jane.doe@company.com", "Jane Doe", "data:image/png;base64,XYZ789", "ANOTHERSECRET")]
    [InlineData("admin@test.org", "Administrator", "data:image/png;base64,ADMINQR", "ADMINSECRET")]
    [InlineData("user.name+tag@domain.co.uk", "User Name", "data:image/png;base64,LONGQR", "VERYLONGSECRET")]
    public async Task HandleEventAsync_WithVariousInputs_ShouldPassCorrectParametersToTemplateGenerator(
        string email, string name, string qrSrc, string manualQrCode)
    {
        // Arrange
        var googleSetupEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = email,
            QrSrc = qrSrc,
            Name = name,
            ManualQrCode = manualQrCode
        };

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(googleSetupEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            name,
            email,
            qrSrc,
            manualQrCode,
            "Two-Factor Setup"), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithEmptyName_ShouldUseEmptyString()
    {
        // Arrange
        var googleSetupEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            QrSrc = "data:image/png;base64,testdata",
            Name = "",
            ManualQrCode = "TESTCODE123"
        };

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(googleSetupEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            "",
            "test@example.com",
            "data:image/png;base64,testdata",
            "TESTCODE123",
            "Two-Factor Setup"), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithLongQrCode_ShouldHandleCorrectly()
    {
        // Arrange
        var longQrCode = "VERYLONGMANUALSECRETCODETHATSHOULDSTILLBEHANDLEDCORRECTLYBYTHETEMPLATEGEN";
        var googleSetupEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            QrSrc = "data:image/png;base64,verylongbase64encodedqrimagedatathatrepresentsaqrcode",
            Name = "Test User",
            ManualQrCode = longQrCode
        };

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(googleSetupEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            "Test User",
            "test@example.com",
            "data:image/png;base64,verylongbase64encodedqrimagedatathatrepresentsaqrcode",
            longQrCode,
            "Two-Factor Setup"), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_MultipleCallsWithDifferentEvents_ShouldHandleEachCorrectly()
    {
        // Arrange
        var firstEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "user1@example.com",
            QrSrc = "data:image/png;base64,QR1DATA",
            Name = "User One",
            ManualQrCode = "SECRET1"
        };

        var secondEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "user2@example.com",
            QrSrc = "data:image/png;base64,QR2DATA",
            Name = "User Two",
            ManualQrCode = "SECRET2"
        };

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(firstEvent);
        await _consumer.HandleEventAsync(secondEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            "User One",
            "user1@example.com",
            "data:image/png;base64,QR1DATA",
            "SECRET1",
            "Two-Factor Setup"), Times.Once);

        _templateGeneratorMock.Verify(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            "User Two",
            "user2@example.com",
            "data:image/png;base64,QR2DATA",
            "SECRET2",
            "Two-Factor Setup"), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(_emailDetailsMock.Object), Times.Exactly(2));
    }

    [Fact]
    public async Task HandleEventAsync_WithSpecialCharactersInParameters_ShouldHandleCorrectly()
    {
        // Arrange
        var googleSetupEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test+special@example.com",
            QrSrc = "data:image/png;base64,ABC123+/=",
            Name = "Test User (Special)",
            ManualQrCode = "SECRET_WITH-SPECIAL@CHARS"
        };

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(googleSetupEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            "Test User (Special)",
            "test+special@example.com",
            "data:image/png;base64,ABC123+/=",
            "SECRET_WITH-SPECIAL@CHARS",
            "Two-Factor Setup"), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithMinimalValidData_ShouldProcessSuccessfully()
    {
        // Arrange
        var googleSetupEvent = new TwoFactorGoogleSetupRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "a@b.c",
            QrSrc = "data:image/png;base64,A",
            Name = "U",
            ManualQrCode = "S"
        };

        _templateGeneratorMock.Setup(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(googleSetupEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateTwoFactorGoogleAuthTemplateAsync(
            "U",
            "a@b.c",
            "data:image/png;base64,A",
            "S",
            "Two-Factor Setup"), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(_emailDetailsMock.Object), Times.Once);
    }
}
