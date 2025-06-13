using Id.Tests.Utility.Exceptions;
using ID.Email.Base.Abs;
using ID.Email.Base.EventListeners.Subscriptions;
using ID.GlobalSettings.Errors;
using ID.IntegrationEvents.Events.Account.Subscriptions;
using Microsoft.Extensions.Logging;
using MyResults;

namespace ID.Email.Base.Tests.EventListeners.Subscriptions;

public class SubscriptionsPausedConsumerTests
{
    private readonly Mock<ILogger<SubscriptionsPausedConsumer>> _loggerMock;
    private readonly Mock<IEmailDetailsTemplateGenerator> _templateGeneratorMock;
    private readonly Mock<IIdEmailService> _emailServiceMock;
    private readonly Mock<IEmailDetails> _emailDetailsMock;
    private readonly SubscriptionsPausedConsumer _consumer;

    public SubscriptionsPausedConsumerTests()
    {
        _loggerMock = new Mock<ILogger<SubscriptionsPausedConsumer>>();
        _templateGeneratorMock = new Mock<IEmailDetailsTemplateGenerator>();
        _emailServiceMock = new Mock<IIdEmailService>();
        _emailDetailsMock = new Mock<IEmailDetails>();

        _consumer = new SubscriptionsPausedConsumer(
            _templateGeneratorMock.Object,
            _emailServiceMock.Object,
            _loggerMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WithValidEvent_ShouldGenerateTemplateAndSendEmail()
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "test@example.com",
            ToName = "Test User",
            SubscriptionPlanName = "Premium Plan"
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            "Test User",
            "test@example.com",
            "Premium Plan",
            "Subscription Paused"), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(_emailDetailsMock.Object), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WithDifferentSubscriptionPlan_ShouldUseCorrectParameters()
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "jane@example.com",
            ToName = "Jane Doe",
            SubscriptionPlanName = "Basic Plan"
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            "Jane Doe",
            "jane@example.com",
            "Basic Plan",
            "Subscription Paused"), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenTemplateGenerationFails_ShouldLogException()
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "test@example.com",
            ToName = "Test User",
            SubscriptionPlanName = "Premium Plan"
        };

        var exception = new Exception("Template generation failed");

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()), Times.Never);

        ExceptionUtils.VerifyExceptionLogging(_loggerMock, IdErrorEvents.Email.TwoFactor, exception);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceReturnsFailure_ShouldLogError()
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "test@example.com",
            ToName = "Test User",
            SubscriptionPlanName = "Premium Plan"
        };

        var errorMessage = "Email service failed";
        var failureResult = BasicResult.Failure(errorMessage);

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(failureResult);

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        ExceptionUtils.VerifyBasicResultLogging(_loggerMock, IdErrorEvents.Email.TwoFactor, failureResult);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceThrowsException_ShouldLogException()
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "test@example.com",
            ToName = "Test User",
            SubscriptionPlanName = "Premium Plan"
        };

        var exception = new Exception("Email service exception");

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ThrowsAsync(exception);

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_loggerMock, IdErrorEvents.Email.TwoFactor, exception);
    }

    [Theory]
    [InlineData("test@example.com", "Test User", "Premium Plan")]
    [InlineData("jane.doe@company.com", "Jane Doe", "Basic Plan")]
    [InlineData("admin@test.org", "Administrator", "Enterprise Plan")]
    [InlineData("user.name+tag@domain.co.uk", "User Name", "Starter Plan")]
    [InlineData("long.email.address@verylongdomainname.organization", "Very Long User Name", "Professional Plus Plan")]
    public async Task HandleEventAsync_WithVariousInputs_ShouldPassCorrectParametersToTemplateGenerator(
        string email, string toName, string subscriptionPlanName)
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = email,
            ToName = toName,
            SubscriptionPlanName = subscriptionPlanName
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            toName,
            email,
            subscriptionPlanName,
            "Subscription Paused"), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WithEmptyToName_ShouldUseEmptyString()
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "test@example.com",
            ToName = "",
            SubscriptionPlanName = "Premium Plan"
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            "",
            "test@example.com",
            "Premium Plan",
            "Subscription Paused"), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WithEmptySubscriptionPlanName_ShouldUseEmptyString()
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "test@example.com",
            ToName = "Test User",
            SubscriptionPlanName = ""
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            "Test User",
            "test@example.com",
            "",
            "Subscription Paused"), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WithSpecialCharactersInParameters_ShouldHandleCorrectly()
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "test+special@example.com",
            ToName = "Test User (Special)",
            SubscriptionPlanName = "Premium Plan & Extra Features"
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            "Test User (Special)",
            "test+special@example.com",
            "Premium Plan & Extra Features",
            "Subscription Paused"), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_MultipleCallsWithDifferentEvents_ShouldHandleEachCorrectly()
    {
        // Arrange
        var firstEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "user1@example.com",
            ToName = "User One",
            SubscriptionPlanName = "Basic Plan"
        };

        var secondEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "user2@example.com",
            ToName = "User Two",
            SubscriptionPlanName = "Premium Plan"
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(firstEvent);
        await _consumer.HandleEventAsync(secondEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            "User One",
            "user1@example.com",
            "Basic Plan",
            "Subscription Paused"), Times.Once);

        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            "User Two",
            "user2@example.com",
            "Premium Plan",
            "Subscription Paused"), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(_emailDetailsMock.Object), Times.Exactly(2));
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WithLongSubscriptionPlanName_ShouldHandleCorrectly()
    {
        // Arrange
        var longPlanName = "Very Long Subscription Plan Name With Many Words And Details About Features Included";
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "test@example.com",
            ToName = "Test User",
            SubscriptionPlanName = longPlanName
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            "Test User",
            "test@example.com",
            longPlanName,
            "Subscription Paused"), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WithMinimalValidData_ShouldProcessSuccessfully()
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "a@b.c",
            ToName = "U",
            SubscriptionPlanName = "P"
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            "U",
            "a@b.c",
            "P",
            "Subscription Paused"), Times.Once);

        _emailServiceMock.Verify(x => x.SendEmailAsync(_emailDetailsMock.Object), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_AlwaysUsesDefaultSubject_RegardlessOfInput()
    {
        // Arrange
        var subscriptionPausedEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = Guid.NewGuid(),
            SubscriptionId = Guid.NewGuid(),
            Email = "test@example.com",
            ToName = "Test User",
            SubscriptionPlanName = "Premium Plan"
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(subscriptionPausedEvent);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            "Subscription Paused"), Times.Once); // Should always be "Subscription Paused"
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WithDifferentGuids_ShouldNotAffectEmailGeneration()
    {
        // Arrange
        var leaderId1 = Guid.NewGuid();
        var subscriptionId1 = Guid.NewGuid();
        var leaderId2 = Guid.NewGuid();
        var subscriptionId2 = Guid.NewGuid();

        var firstEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = leaderId1,
            SubscriptionId = subscriptionId1,
            Email = "test@example.com",
            ToName = "Test User",
            SubscriptionPlanName = "Premium Plan"
        };

        var secondEvent = new SubscriptionsPausedIntegrationEvent
        {
            LeaderId = leaderId2,
            SubscriptionId = subscriptionId2,
            Email = "test@example.com",
            ToName = "Test User",
            SubscriptionPlanName = "Premium Plan"
        };

        _templateGeneratorMock.Setup(x => x.GenerateSubscriptionPausedTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(firstEvent);
        await _consumer.HandleEventAsync(secondEvent);

        // Assert - Both should generate identical template calls since GUIDs don't affect email content
        _templateGeneratorMock.Verify(x => x.GenerateSubscriptionPausedTemplateAsync(
            "Test User",
            "test@example.com",
            "Premium Plan",
            "Subscription Paused"), Times.Exactly(2));
    }

}//Cls
