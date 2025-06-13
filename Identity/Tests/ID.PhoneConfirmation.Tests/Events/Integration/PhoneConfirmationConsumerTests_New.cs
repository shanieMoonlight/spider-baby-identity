using ID.Application.AppAbs.Messaging;
using ID.GlobalSettings.Errors;
using ID.GlobalSettings.Setup.Options;
using ID.IntegrationEvents.Events.Account.PhoneConfirmation;
using ID.PhoneConfirmation.Events.Integration;
using ID.PhoneConfirmation.Tests.Utils;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyResults;
using Shouldly;

namespace ID.PhoneConfirmation.Tests.Events.Integration;

public class PhoneConfirmationConsumerTests_New
{
    private readonly Mock<ILogger<PhoneConfirmationConsumer>> _loggerMock;
    private readonly Mock<IIdSmsService> _smsServiceMock;
    private readonly Mock<IOptions<IdGlobalOptions>> _optionsMock;
    private readonly Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>> _customerOptionsMock;
    private readonly PhoneConfirmationConsumer _consumer;
    private readonly IdGlobalOptions _globalOptions;
    private readonly IdGlobalSetupOptions_CUSTOMER _customerOptions;

    public PhoneConfirmationConsumerTests_New()
    {
        _loggerMock = new Mock<ILogger<PhoneConfirmationConsumer>>();
        _smsServiceMock = new Mock<IIdSmsService>();
        _optionsMock = new Mock<IOptions<IdGlobalOptions>>();
        _customerOptionsMock = new Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>>();
        
        _globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            mntcAccountsUrl: "https://maintenance.example.com",
            applicationName: "Test Application",
            defaultMaxTeamPosition: 10,
            defaultMinTeamPosition: 1,
            superTeamMinPosition: 1,
            superTeamMaxPosition: 10,
            claimTypePrefix: "test",
            refreshTokensEnabled: true,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(15)
        );
        
        _customerOptions = GlobalOptionsUtils.InitiallyValidCustomerOptions(
            customerAccountsUrl: "https://customer.example.com"
        );
        
        _optionsMock.Setup(x => x.Value).Returns(_globalOptions);
        _customerOptionsMock.Setup(x => x.Value).Returns(_customerOptions);
        
        _consumer = new PhoneConfirmationConsumer(
            _smsServiceMock.Object,
            _loggerMock.Object,
            _optionsMock.Object,
            _customerOptionsMock.Object);
    }

    [Fact]
    public async Task HandleEventAsync_WithValidCustomerEvent_ShouldSendSmsWithCustomerUrl()
    {
        // Arrange
        var phoneConfirmationEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Phone = "1234567890",
            ConfirmationToken = "ABC123",
            IsCustomerTeam = true
        };

        _smsServiceMock.Setup(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(phoneConfirmationEvent);

        // Assert
        _smsServiceMock.Verify(x => x.SendMsgAsync(
            "1234567890",
            It.Is<string>(msg => 
                msg.Contains("testuser") && 
                msg.Contains("Test Application") &&
                msg.Contains("https://customer.example.com") &&
                msg.Contains(phoneConfirmationEvent.UserId.ToString()) &&
                msg.Contains("ABC123") &&
                msg.Contains("text message"))), 
            Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithValidMaintenanceEvent_ShouldSendSmsWithMaintenanceUrl()
    {
        // Arrange
        var phoneConfirmationEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Username = "maintenanceuser",
            Phone = "0987654321",
            ConfirmationToken = "XYZ789",
            IsCustomerTeam = false
        };

        _smsServiceMock.Setup(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(phoneConfirmationEvent);

        // Assert
        _smsServiceMock.Verify(x => x.SendMsgAsync(
            "0987654321",
            It.Is<string>(msg => 
                msg.Contains("maintenanceuser") && 
                msg.Contains("Test Application") &&
                msg.Contains("https://maintenance.example.com") &&
                msg.Contains(phoneConfirmationEvent.UserId.ToString()) &&
                msg.Contains("XYZ789") &&
                msg.Contains("text message"))), 
            Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithNullPhone_ShouldNotSendSms()
    {
        // Arrange
        var phoneConfirmationEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Phone = null!,
            ConfirmationToken = "ABC123",
            IsCustomerTeam = true
        };

        // Act
        await _consumer.HandleEventAsync(phoneConfirmationEvent);

        // Assert
        _smsServiceMock.Verify(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleEventAsync_WithEmptyPhone_ShouldNotSendSms()
    {
        // Arrange
        var phoneConfirmationEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Phone = "",
            ConfirmationToken = "ABC123",
            IsCustomerTeam = true
        };

        // Act
        await _consumer.HandleEventAsync(phoneConfirmationEvent);

        // Assert
        _smsServiceMock.Verify(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleEventAsync_WithWhitespacePhone_ShouldNotSendSms()
    {
        // Arrange
        var phoneConfirmationEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Phone = "   ",
            ConfirmationToken = "ABC123",
            IsCustomerTeam = true
        };

        // Act
        await _consumer.HandleEventAsync(phoneConfirmationEvent);

        // Assert
        _smsServiceMock.Verify(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleEventAsync_WhenSmsServiceReturnsFailure_ShouldLogError()
    {
        // Arrange
        var phoneConfirmationEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Phone = "1234567890",
            ConfirmationToken = "ABC123",
            IsCustomerTeam = true
        };

        var errorMessage = "SMS service failed";
        var failureResult = BasicResult.Failure(errorMessage);
        _smsServiceMock.Setup(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(failureResult);

        // Act
        await _consumer.HandleEventAsync(phoneConfirmationEvent);

        // Assert
        ExceptionUtils.VerifyBasicResultLogging(_loggerMock, IdErrorEvents.Email.PhoneConfirmation, failureResult);
    }

    [Fact]
    public async Task HandleEventAsync_WhenSmsServiceThrowsException_ShouldLogException()
    {
        // Arrange
        var phoneConfirmationEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Phone = "1234567890",
            ConfirmationToken = "ABC123",
            IsCustomerTeam = true
        };

        var exception = new Exception("SMS service exception");
        _smsServiceMock.Setup(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(exception);

        // Act
        await _consumer.HandleEventAsync(phoneConfirmationEvent);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_loggerMock, IdErrorEvents.Email.PhoneConfirmation, exception);
    }

    [Theory]
    [InlineData("testuser", "1234567890", "ABC123", true)]
    [InlineData("maintenanceuser", "0987654321", "XYZ789", false)]
    [InlineData("user with spaces", "5555555555", "CODE123", true)]
    public async Task HandleEventAsync_WithValidData_ShouldGenerateCorrectMessage(
        string username, string phone, string token, bool isCustomerTeam)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var phoneConfirmationEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = userId,
            Username = username,
            Phone = phone,
            ConfirmationToken = token,
            IsCustomerTeam = isCustomerTeam
        };

        _smsServiceMock.Setup(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Success());

        var expectedBaseUrl = isCustomerTeam ? _customerOptions.CustomerAccountsUrl : _globalOptions.MntcAccountsUrl;
        var expectedUrl = $"{expectedBaseUrl}/Account/ConfirmPhone?userId={userId}&confirmationToken={token}";

        // Act
        await _consumer.HandleEventAsync(phoneConfirmationEvent);

        // Assert
        _smsServiceMock.Verify(x => x.SendMsgAsync(
            phone,
            It.Is<string>(msg => 
                msg.Contains(username) && 
                msg.Contains(_globalOptions.ApplicationName) &&
                msg.Contains(userId.ToString()) &&
                msg.Contains(token) &&
                msg.Contains("text message") &&
                msg.Contains("Hi " + username) &&
                msg.Contains("Thanks") &&
                msg.Contains("The Team at " + _globalOptions.ApplicationName))), 
            Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithSpecialCharactersInParameters_ShouldHandleCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var phoneConfirmationEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = userId,
            Username = "test&user",
            Phone = "+1-234-567-8900",
            ConfirmationToken = "ABC&123",
            IsCustomerTeam = true
        };

        _smsServiceMock.Setup(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(phoneConfirmationEvent);

        // Assert
        _smsServiceMock.Verify(x => x.SendMsgAsync(
            "+1-234-567-8900",
            It.Is<string>(msg => 
                msg.Contains("test&user") && 
                msg.Contains(userId.ToString()) &&
                msg.Contains("ABC&123"))), 
            Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_WithNullUsername_ShouldUseEmptyString()
    {
        // Arrange
        var phoneConfirmationEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Username = null!,
            Phone = "1234567890",
            ConfirmationToken = "ABC123",
            IsCustomerTeam = true
        };

        _smsServiceMock.Setup(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(phoneConfirmationEvent);

        // Assert
        _smsServiceMock.Verify(x => x.SendMsgAsync(
            "1234567890",
            It.Is<string>(msg => 
                msg.Contains("Hi ,") && // Empty username should result in "Hi ,"
                msg.Contains(_globalOptions.ApplicationName))), 
            Times.Once);
    }

    [Fact]
    public async Task HandleEventAsync_MultipleCallsWithDifferentEvents_ShouldHandleEachCorrectly()
    {
        // Arrange
        var customerEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Username = "customer",
            Phone = "1111111111",
            ConfirmationToken = "CUST123",
            IsCustomerTeam = true
        };

        var maintenanceEvent = new PhoneConfirmationIntegrationEvent
        {
            UserId = Guid.NewGuid(),
            Username = "maintenance",
            Phone = "2222222222",
            ConfirmationToken = "MAINT456",
            IsCustomerTeam = false
        };

        _smsServiceMock.Setup(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(customerEvent);
        await _consumer.HandleEventAsync(maintenanceEvent);

        // Assert
        _smsServiceMock.Verify(x => x.SendMsgAsync(
            "1111111111",
            It.Is<string>(msg => 
                msg.Contains("customer") && 
                msg.Contains(_globalOptions.ApplicationName) &&
                msg.Contains(customerEvent.UserId.ToString()) &&
                msg.Contains("CUST123"))), 
            Times.Once);

        _smsServiceMock.Verify(x => x.SendMsgAsync(
            "2222222222",
            It.Is<string>(msg => 
                msg.Contains("maintenance") && 
                msg.Contains(_globalOptions.ApplicationName) &&
                msg.Contains(maintenanceEvent.UserId.ToString()) &&
                msg.Contains("MAINT456"))), 
            Times.Once);

        _smsServiceMock.Verify(x => x.SendMsgAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public void CreateConfirmationMsgCustomer_ShouldGenerateCorrectMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "testuser";
        var token = "ABC123";

        // Act
        var result = _consumer.CreateConfirmationMsgCustomer(userId, username, token);

        // Assert
        result.ShouldContain(username);
        result.ShouldContain(_globalOptions.ApplicationName);
        result.ShouldContain(userId.ToString());
        result.ShouldContain(token);
        result.ShouldContain(_customerOptions.CustomerAccountsUrl);
        result.ShouldContain("text message");
    }

    [Fact]
    public void CreateConfirmationMsgMntc_ShouldGenerateCorrectMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "maintenanceuser";
        var token = "XYZ789";

        // Act
        var result = _consumer.CreateConfirmationMsgMntc(userId, username, token);

        // Assert
        result.ShouldContain(username);
        result.ShouldContain(_globalOptions.ApplicationName);
        result.ShouldContain(userId.ToString());
        result.ShouldContain(token);
        result.ShouldContain(_globalOptions.MntcAccountsUrl);
        result.ShouldContain("text message");
    }
}
