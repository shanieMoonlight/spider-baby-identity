using ID.Email.Base.EventListeners.TrustedDevices;
using ID.Email.Base.LocalAbs;
using ID.Email.Base.LocalImps.Specs.TrustedDevices;
using ID.GlobalSettings.Errors;
using ID.GlobalSettings.Setup.Options;
using ID.IntegrationEvents.Events.Account.TrustedDevices;
using ID.Tests.Data.GlobalOptions;
using ID.Tests.Utility.Logging;
using Microsoft.Extensions.Logging;

namespace ID.Email.Base.Tests.EventListeners.TrustedDevices;

public class TrustedDeviceAddedConsumerTests
{
    private readonly Mock<ILogger<TrustedDeviceAddedConsumer>> _loggerMock;
    private readonly Mock<IEmailDetailsTemplateGenerator> _templateGeneratorMock;
    private readonly Mock<IIdEmailService> _emailServiceMock;
    private readonly Mock<IEmailDetails> _emailDetailsMock;
    private readonly TrustedDeviceAddedConsumer _consumer;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions;
    private readonly Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>> _mockCustomerOptions;


    public TrustedDeviceAddedConsumerTests()
    {
        // Setup options
        IdGlobalOptions  _globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test Application",
            mntcAccountsUrl: "https://mntc.example.com/accounts",
            defaultMaxTeamPosition: 10,
            defaultMinTeamPosition: 1,
            superTeamMinPosition: 1,
            superTeamMaxPosition: 10,
            claimTypePrefix: "test_claim",
            refreshTokensEnabled: true,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(15)
        );

        IdGlobalSetupOptions_CUSTOMER _customerOptions = GlobalOptionsUtils.InitiallyValidCustomerOptions(
            customerAccountsUrl: "https://customer.example.com/accounts",
            maxTeamPosition: 5,
            minTeamPosition: 1,
            maxTeamSize: 20
        );

        _loggerMock = new Mock<ILogger<TrustedDeviceAddedConsumer>>();
        _templateGeneratorMock = new Mock<IEmailDetailsTemplateGenerator>();
        _emailServiceMock = new Mock<IIdEmailService>();
        _emailDetailsMock = new Mock<IEmailDetails>();
        _mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        _mockCustomerOptions = new Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>>();

        _mockGlobalOptions.Setup(x => x.Value).Returns(_globalOptions);
        _mockCustomerOptions.Setup(x => x.Value).Returns(_customerOptions);

        _consumer = new TrustedDeviceAddedConsumer(
            _templateGeneratorMock.Object,
            _emailServiceMock.Object,
            _mockGlobalOptions.Object,
            _mockCustomerOptions.Object,
            _loggerMock.Object);
    }

    //----------------////

    [Fact]
    public async Task HandleEventAsync_WithValidEvent_ShouldGenerateTemplateAndSendEmail()
    {
        // Arrange
        var evt = new TrustedDeviceAddedIntegrationEvent
        {
            UserEmail = "test@example.com",
            UserName = "Test User",
            UserAgent = "UA",
            IpAddress = "127.0.0.1",
            DeviceName = "Phone",
            IsCustomerTeam = false,
            DateAdded = DateTime.UtcNow
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<TrustedDeviceAddedSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _consumer.HandleEventAsync(evt);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TrustedDeviceAddedSpec>()), Times.Once);
        _emailServiceMock.Verify(x => x.SendEmailAsync(_emailDetailsMock.Object), Times.Once);
    }

    //----------------////

    [Fact]
    public async Task HandleEventAsync_WhenTemplateGenerationFails_ShouldLogAndNotSendEmail()
    {
        // Arrange
        var evt = new TrustedDeviceAddedIntegrationEvent
        {
            UserEmail = "test@example.com",
            UserName = "Test User",
            UserAgent = "UA",
            IpAddress = "127.0.0.1",
            DeviceName = "Phone",
            IsCustomerTeam = true,
            DateAdded = DateTime.UtcNow
        };

        var ex = new Exception("template fail");
        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<TrustedDeviceAddedSpec>()))
            .ThrowsAsync(ex);

        // Act
        await _consumer.HandleEventAsync(evt);

        // Assert
        _templateGeneratorMock.Verify(x => x.GenerateFromSpecAsync(It.IsAny<TrustedDeviceAddedSpec>()), Times.Once);
        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()), Times.Never);
        LoggingUtils.VerifyExceptionLogging(_loggerMock, IdErrorEvents.Email.ForgotPassword, ex);
    }

    //----------------////

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceFails_ShouldLogBasicResultFailure()
    {
        // Arrange
        var evt = new TrustedDeviceAddedIntegrationEvent
        {
            UserEmail = "test@example.com",
            UserName = "Test User",
            UserAgent = "UA",
            IpAddress = "127.0.0.1",
            DeviceName = "Phone",
            IsCustomerTeam = false,
            DateAdded = DateTime.UtcNow
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<TrustedDeviceAddedSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        var fail = BasicResult.Failure("send fail");
        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ReturnsAsync(fail);

        // Act
        await _consumer.HandleEventAsync(evt);

        // Assert
        LoggingUtils.VerifyBasicResultLogging(_loggerMock, IdErrorEvents.Email.TrustedDevices, fail);
    }

    //----------------////

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceThrows_ShouldLogException()
    {
        // Arrange
        var evt = new TrustedDeviceAddedIntegrationEvent
        {
            UserEmail = "test@example.com",
            UserName = "Test User",
            UserAgent = "UA",
            IpAddress = "127.0.0.1",
            DeviceName = "Phone",
            IsCustomerTeam = false,
            DateAdded = DateTime.UtcNow
        };

        _templateGeneratorMock.Setup(x => x.GenerateFromSpecAsync(It.IsAny<TrustedDeviceAddedSpec>()))
            .ReturnsAsync(_emailDetailsMock.Object);

        var ex = new Exception("send exception");
        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<IEmailDetails>()))
            .ThrowsAsync(ex);

        // Act
        await _consumer.HandleEventAsync(evt);

        // Assert
        LoggingUtils.VerifyExceptionLogging(_loggerMock, IdErrorEvents.Email.ForgotPassword, ex);
    }

}
