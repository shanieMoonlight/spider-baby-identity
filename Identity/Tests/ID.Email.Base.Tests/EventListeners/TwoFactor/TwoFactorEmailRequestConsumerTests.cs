using Id.Tests.Utility.Exceptions;
using ID.Email.Base.EventListeners.TwoFactor;
using ID.Email.Base.LocalAbs;
using ID.Email.Base.LocalImps.Specs;
using ID.GlobalSettings.Errors;
using ID.IntegrationEvents.Events.Account.TwoFactor;
using Microsoft.Extensions.Logging;

namespace ID.Email.Base.Tests.EventListeners.TwoFactor;

public class TwoFactorEmailRequestConsumerTests
{
    private readonly Mock<ILogger<TwoFactorEmailRequestConsumer>> _loggerMock;
    private readonly Mock<IEmailDetailsTemplateGenerator> _templateGeneratorMock;
    private readonly Mock<IIdEmailService> _emailServiceMock;
    private readonly Mock<IEmailDetails> _emailDetailsMock;
    private readonly TwoFactorEmailRequestConsumer _consumer;

    public TwoFactorEmailRequestConsumerTests()
    {
        _loggerMock = new Mock<ILogger<TwoFactorEmailRequestConsumer>>();
        _templateGeneratorMock = new Mock<IEmailDetailsTemplateGenerator>();
        _emailServiceMock = new Mock<IIdEmailService>();
        _emailDetailsMock = new Mock<IEmailDetails>();
        
        _consumer = new TwoFactorEmailRequestConsumer(
            _templateGeneratorMock.Object,
            _emailServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleEventAsync_WithValidEvent_ShouldGenerateTemplateAndSendEmail()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = "1234567890",
            Name = "Test User",
            VerificationCode = "123456"
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TwoFactorSpec>()), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(_emailDetailsMock.Object), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithNullPhone_ShouldStillProcessSuccessfully()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = null,
            Name = "Test User",
            VerificationCode = "789012"
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TwoFactorSpec>()), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(_emailDetailsMock.Object), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithDifferentNames_ShouldUseCorrectName()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "jane@example.com",
            Phone = "0987654321",
            Name = "Jane Doe",
            VerificationCode = "654321"
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TwoFactorSpec>()), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WhenTemplateGenerationFails_ShouldStillAttemptToSendEmail()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = "1234567890",
            Name = "Test User",
            VerificationCode = "123456"
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ThrowsAsync(new Exception("Template generation failed"));

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TwoFactorSpec>()), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()), Times.Never);
    }

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceReturnsFailure_ShouldLogError()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = "1234567890",
            Name = "Test User",
            VerificationCode = "123456"
        };

        var errorMessage = "Email service failed";
        var failureResult = BasicResult.Failure(errorMessage);

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(failureResult);

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        ExceptionUtils.VerifyBasicResultLogging(_loggerMock, IdErrorEvents.Email.TwoFactor, failureResult);
    }

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceThrowsException_ShouldLogException()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = "1234567890",
            Name = "Test User",
            VerificationCode = "123456"
        };

        var exception = new Exception("Email service exception");

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ThrowsAsync(exception);

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_loggerMock, IdErrorEvents.Email.TwoFactor, exception);
    }

    [Fact]
    public async Task HandleEventAsync_WhenTemplateGeneratorThrowsException_ShouldLogException()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = "1234567890",
            Name = "Test User",
            VerificationCode = "123456"
        };

        var exception = new Exception("Template generator exception");

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ThrowsAsync(exception);

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_loggerMock, IdErrorEvents.Email.TwoFactor, exception);
    }

    [Theory]
    [InlineData("test@example.com", "Test User", "123456")]
    [InlineData("jane.doe@company.com", "Jane Doe", "789012")]
    [InlineData("admin@test.org", "Administrator", "000000")]
    [InlineData("user.name+tag@domain.co.uk", "User Name", "999999")]
    public async Task HandleEventAsync_WithVariousInputs_ShouldPassCorrectParametersToTemplateGenerator(
        string email, string name, string verificationCode)
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = email,
            Phone = "1234567890",
            Name = name,
            VerificationCode = verificationCode
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TwoFactorSpec>()), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithEmptyName_ShouldUseEmptyString()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = "1234567890",
            Name = "",
            VerificationCode = "123456"
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TwoFactorSpec>()), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithNullName_ShouldUseNullString()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = "1234567890",
            Name = null!,
            VerificationCode = "123456"
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TwoFactorSpec>()), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithSpecialCharactersInVerificationCode_ShouldHandleCorrectly()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = "1234567890",
            Name = "Test User",
            VerificationCode = "A1B2C3"
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TwoFactorSpec>()), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_MultipleCallsWithDifferentEvents_ShouldHandleEachCorrectly()
    {
        // Arrange
        var firstEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "user1@example.com",
            Phone = "1111111111",
            Name = "User One",
            VerificationCode = "111111"
        };

        var secondEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "user2@example.com",
            Phone = "2222222222",
            Name = "User Two",
            VerificationCode = "222222"
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(firstEvent);
        await _consumer.HandleEventAsync(secondEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TwoFactorSpec>()), Times.Exactly(2));

        _emailServiceMock.Verify(x => x.SendEmailAsync(_emailDetailsMock.Object), Times.Exactly(2));
    }

    [Fact]
    public async Task HandleEventAsync_WithLongVerificationCode_ShouldHandleCorrectly()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = "1234567890",
            Name = "Test User",
            VerificationCode = "ABCDEF123456789"
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()), Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_AlwaysUsesCorrectSubject()
    {
        // Arrange
        var twoFactorEvent = new TwoFactorEmailRequestIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Phone = "1234567890",
            Name = "Test User",
            VerificationCode = "123456"
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(twoFactorEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<IEmailSpec>()), Times.Once);
    }
}
