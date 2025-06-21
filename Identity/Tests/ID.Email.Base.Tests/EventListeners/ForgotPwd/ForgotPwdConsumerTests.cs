using Id.Tests.Utility.Exceptions;
using ID.Email.Base.Abs;
using ID.Email.Base.EventListeners.ForgotPwd;
using ID.GlobalSettings.Constants;
using ID.GlobalSettings.Errors;
using ID.GlobalSettings.Setup.Options;
using ID.GlobalSettings.Utility;
using ID.IntegrationEvents.Events.Account.ForgotPwd;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyResults;

namespace ID.Email.Base.Tests.EventListeners.ForgotPwd;

public class ForgotPwdConsumerTests
{
    private readonly Mock<IEmailDetailsTemplateGenerator> _mockTemplateGenerator;
    private readonly Mock<IIdEmailService> _mockEmailService;
    private readonly Mock<ILogger<ForgotPwdConsumer>> _mockLogger;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions;
    private readonly Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>> _mockCustomerOptions;
    private readonly IdGlobalOptions _globalOptions;
    private readonly IdGlobalSetupOptions_CUSTOMER _customerOptions;
    private readonly ForgotPwdConsumer _consumer;

    //------------------------------------//

    public ForgotPwdConsumerTests()
    {
        // Setup options
        _globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
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

        _customerOptions = GlobalOptionsUtils.InitiallyValidCustomerOptions(
            customerAccountsUrl: "https://customer.example.com/accounts",
            maxTeamPosition: 5,
            minTeamPosition: 1,
            maxTeamSize: 20
        );

        // Setup mocks
        _mockTemplateGenerator = new Mock<IEmailDetailsTemplateGenerator>();
        _mockEmailService = new Mock<IIdEmailService>();
        _mockLogger = new Mock<ILogger<ForgotPwdConsumer>>();
        _mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        _mockCustomerOptions = new Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>>();

        _mockGlobalOptions.Setup(x => x.Value).Returns(_globalOptions);
        _mockCustomerOptions.Setup(x => x.Value).Returns(_customerOptions);

        // Create consumer
        _consumer = new ForgotPwdConsumer(
            _mockTemplateGenerator.Object,
            _mockEmailService.Object,
            _mockGlobalOptions.Object,
            _mockCustomerOptions.Object,
            _mockLogger.Object);
    }

    //------------------------------------//

    private static ForgotPwdEmailRequestIntegrationEvent CreateTestEventData(bool isCustomerTeam = true)
    {
        return new ForgotPwdEmailRequestIntegrationEvent(
            Guid.Parse("c6f88c01-f4a2-48a6-ab6d-d3865d9974e8"),
            "test@example.com",
            "1234567890",
            "Test User",
            "test-reset-token",
            isCustomerTeam);
    }

    private static EmailDetails CreateTestEmailDetails()
    {
        return new EmailDetails(
            type: EmailType.HTML,
            message: "<html>Test Content</html>",
            subject: "Test Subject",
            toAddresses: ["test@example.com"],
            bccAddresses: [],
            fromAddress: "sender@example.com",
            fromName: "Test Sender");
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenCustomerTeam_ShouldUseCustomerAccountsUrl()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_customerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ResetPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ResetToken}={eventData.ResetToken}";

        _mockTemplateGenerator
            .Setup(x => x.GeneratePasswordResetTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GeneratePasswordResetTemplateAsync(
            eventData.Name, eventData.Email, expectedUrl), Times.Once);

        _mockEmailService.Verify(x => x.SendEmailAsync(expectedEmailDetails), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenMaintenance_ShouldUseMntcAccountsUrl()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: false);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_globalOptions.MntcAccountsUrl, IdGlobalConstants.EmailRoutes.ResetPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ResetToken}={eventData.ResetToken}";

        _mockTemplateGenerator
            .Setup(x => x.GeneratePasswordResetTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GeneratePasswordResetTemplateAsync(
            eventData.Name, eventData.Email, expectedUrl), Times.Once);

        _mockEmailService.Verify(x => x.SendEmailAsync(expectedEmailDetails), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceFails_ShouldLogFailure()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var failureResult = BasicResult.NotFoundResult("Email service failed");

        var expectedUrl = $"{UrlBuilder.Combine(_customerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ResetPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ResetToken}={eventData.ResetToken}";

        _mockTemplateGenerator
            .Setup(x => x.GeneratePasswordResetTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(failureResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        ExceptionUtils.VerifyBasicResultLogging(_mockLogger, IdErrorEvents.Email.ForgotPassword, failureResult);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenExceptionThrown_ShouldLogException()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedException = new InvalidOperationException("Template generation failed");

        _mockTemplateGenerator
            .Setup(x => x.GeneratePasswordResetTemplateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_mockLogger, IdErrorEvents.Email.ForgotPassword, expectedException);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_ShouldPassCorrectParametersToTemplateGenerator()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_customerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ResetPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ResetToken}={eventData.ResetToken}";

        _mockTemplateGenerator
            .Setup(x => x.GeneratePasswordResetTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GeneratePasswordResetTemplateAsync(
            eventData.Name,
            eventData.Email,
            expectedUrl),
        Times.Once);

        _mockEmailService.Verify(x => x.SendEmailAsync(expectedEmailDetails), Times.Once);
    }

    //------------------------------------//   

    [Fact]
    public async Task HandleEventAsync_ShouldGenerateCorrectResetPasswordUrl_ForCustomer()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_customerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ResetPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ResetToken}={eventData.ResetToken}";

        _mockTemplateGenerator
            .Setup(x => x.GeneratePasswordResetTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GeneratePasswordResetTemplateAsync(
            eventData.Name, eventData.Email, expectedUrl), Times.Once);

        _mockEmailService.Verify(x => x.SendEmailAsync(expectedEmailDetails), Times.Once);
    }

    //------------------------------------//   
    [Fact]
    public async Task HandleEventAsync_ShouldGenerateCorrectResetPasswordUrl_ForMaintenance()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: false);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_globalOptions.MntcAccountsUrl, IdGlobalConstants.EmailRoutes.ResetPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ResetToken}={eventData.ResetToken}";

        _mockTemplateGenerator
            .Setup(x => x.GeneratePasswordResetTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GeneratePasswordResetTemplateAsync(
            eventData.Name, eventData.Email, expectedUrl), Times.Once);

        _mockEmailService.Verify(x => x.SendEmailAsync(expectedEmailDetails), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenTemplateGeneratorFails_ShouldLogException()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedException = new ArgumentException("Invalid template parameters");

        _mockTemplateGenerator
            .Setup(x => x.GeneratePasswordResetTemplateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_mockLogger, IdErrorEvents.Email.ForgotPassword, expectedException);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_ShouldCallEmailServiceOnlyOnce()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        _mockTemplateGenerator
            .Setup(x => x.GeneratePasswordResetTemplateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<EmailDetails>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenEmailServiceReturnsFailure_ShouldNotThrow()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var failureResult = BasicResult.Failure("Email sending failed");

        _mockTemplateGenerator
            .Setup(x => x.GeneratePasswordResetTemplateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(failureResult);

        // Act & Assert - Should not throw
        await _consumer.HandleEventAsync(eventData);

        // Verify failure was logged
        ExceptionUtils.VerifyBasicResultLogging(_mockLogger, IdErrorEvents.Email.ForgotPassword, failureResult);
    }
}
