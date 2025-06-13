using ID.Email.Base.Abs;
using ID.Email.Base.EventListeners.EmailConfirmation;
using Id.Tests.Utility.Exceptions;
using ID.GlobalSettings.Constants;
using ID.GlobalSettings.Setup.Options;
using ID.GlobalSettings.Utility;
using ID.IntegrationEvents.Events.Account.EmailConfirmation;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyResults;
using ID.GlobalSettings.Errors;

namespace ID.Email.Base.Tests.EventListeners.EmailConfirmation;

public class EmailConfirmationRequiringPasswordConsumerTests
{
    private readonly Mock<IEmailDetailsTemplateGenerator> _mockTemplateGenerator;
    private readonly Mock<IIdEmailService> _mockEmailService;
    private readonly Mock<ILogger<EmailConfirmationRequiringPasswordConsumer>> _mockLogger;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions;
    private readonly Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>> _mockCustomerOptions;
    private readonly IdGlobalOptions _globalOptions;
    private readonly IdGlobalSetupOptions_CUSTOMER _customerOptions;
    private readonly EmailConfirmationRequiringPasswordConsumer _consumer;

    //------------------------------------//

    public EmailConfirmationRequiringPasswordConsumerTests()
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
        _mockLogger = new Mock<ILogger<EmailConfirmationRequiringPasswordConsumer>>();
        _mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        _mockCustomerOptions = new Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>>();

        _mockGlobalOptions.Setup(x => x.Value).Returns(_globalOptions);
        _mockCustomerOptions.Setup(x => x.Value).Returns(_customerOptions);

        // Create consumer
        _consumer = new EmailConfirmationRequiringPasswordConsumer(
            _mockTemplateGenerator.Object,
            _mockGlobalOptions.Object,
            _mockCustomerOptions.Object,
            _mockEmailService.Object,
            _mockLogger.Object);    }

    //------------------------------------//

    private static EmailConfirmationRequiringPasswordIntegrationEvent CreateTestEventData(bool isCustomerTeam = true)
    {
        return new EmailConfirmationRequiringPasswordIntegrationEvent(
            Guid.Parse("c6f88c01-f4a2-48a6-ab6d-d3865d9974e8"),
            "test@example.com",
            "1234567890",
            "Test User",
            "test-confirmation-token",
            isCustomerTeam);
    }    private static EmailDetails CreateTestEmailDetails()
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
    public async Task HandleEventAsync_WhenCustomerTeam_ShouldCallSendRegistrationEmailCustomer()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_customerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
            eventData.Name, eventData.Email, expectedUrl), Times.Once);
        
        _mockEmailService.Verify(x => x.SendEmailAsync(expectedEmailDetails), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenMaintenance_ShouldCallSendRegistrationEmailMntc()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: false);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_globalOptions.MntcAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationMntcTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateEmailConfirmationMntcTemplateAsync(
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

        var expectedUrl = $"{UrlBuilder.Combine(_customerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(failureResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        ExceptionUtils.VerifyBasicResultLogging(_mockLogger, IdErrorEvents.Email.EmailConfirmation, failureResult);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenExceptionThrown_ShouldLogException()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedException = new InvalidOperationException("Template generation failed");

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_mockLogger, IdErrorEvents.Email.EmailConfirmation, expectedException);
    }

    //------------------------------------//

    [Fact]
    public async Task SendRegistrationEmailCustomer_ShouldPassCorrectParametersToTemplateGenerator()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_customerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        var result = await _consumer.SendRegistrationEmailCustomer(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
            eventData.Name,
            eventData.Email, 
            expectedUrl), 
        Times.Once);
        
        _mockEmailService.Verify(x => x.SendEmailAsync(expectedEmailDetails), Times.Once);
        
        result.ShouldBe(successResult);
    }

    //------------------------------------//

    [Fact]
    public async Task SendRegistrationEmailMntc_ShouldPassCorrectParametersToTemplateGenerator()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: false);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_globalOptions.MntcAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationMntcTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        var result = await _consumer.SendRegistrationEmailMntc(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateEmailConfirmationMntcTemplateAsync(
            eventData.Name, eventData.Email, expectedUrl), Times.Once);
        
        _mockEmailService.Verify(x => x.SendEmailAsync(expectedEmailDetails), Times.Once);
        
        result.ShouldBe(successResult);
    }

    //------------------------------------//

    [Fact]
    public async Task SendRegistrationEmailCustomer_WhenEmailServiceFails_ShouldReturnFailureResult()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var failureResult = BasicResult.NotFoundResult("Email service failed");

        var expectedUrl = $"{UrlBuilder.Combine(_customerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _consumer.SendRegistrationEmailCustomer(eventData);

        // Assert
        result.ShouldBe(failureResult);
    }

    //------------------------------------//

    [Fact]
    public async Task SendRegistrationEmailMntc_WhenEmailServiceFails_ShouldReturnFailureResult()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: false);
        var expectedEmailDetails = CreateTestEmailDetails();
        var failureResult = BasicResult.NotFoundResult("Email service failed");

        var expectedUrl = $"{UrlBuilder.Combine(_globalOptions.MntcAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationMntcTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _consumer.SendRegistrationEmailMntc(eventData);

        // Assert
        result.ShouldBe(failureResult);
    }

    //------------------------------------//

    [Fact]
    public async Task SendRegistrationEmailCustomer_ShouldGenerateCorrectConfirmEmailWithPasswordUrl()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_customerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.SendRegistrationEmailCustomer(eventData);

        // Assert - Verify the URL contains the correct route and parameters
        _mockTemplateGenerator.Verify(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
            eventData.Name, eventData.Email, expectedUrl), Times.Once);

    }

    //------------------------------------//

    [Fact]
    public async Task SendRegistrationEmailMntc_ShouldGenerateCorrectConfirmEmailWithPasswordUrl()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: false);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_globalOptions.MntcAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmailWithPassword)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationMntcTemplateAsync(
                eventData.Name, eventData.Email, expectedUrl))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        //https://mntc.example.com/accounts/confirm-email-with-password?userid=c6f88c01-f4a2-48a6-ab6d-d3865d9974e8&confirmationtoken=test-confirmation-token
        //https://mntc.example.com/accounts/confirm-email-with-password?userid=c6f88c01-f4a2-48a6-ab6d-d3865d9974e8&confirmationtoken=test-confirmation-token
        // Act
        await _consumer.SendRegistrationEmailMntc(eventData);

        // Assert - Verify the URL contains the correct route and parameters
        _mockTemplateGenerator.Verify(x => x.GenerateEmailConfirmationMntcTemplateAsync(
            eventData.Name, eventData.Email, expectedUrl), Times.Once);
    }
}
