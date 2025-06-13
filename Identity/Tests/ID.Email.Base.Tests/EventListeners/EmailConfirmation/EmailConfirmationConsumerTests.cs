using Id.Tests.Utility.Exceptions;
using ID.Email.Base.Abs;
using ID.Email.Base.EventListeners.EmailConfirmation;
using ID.GlobalSettings.Constants;
using ID.GlobalSettings.Errors;
using ID.GlobalSettings.Setup.Options;
using ID.GlobalSettings.Utility;
using ID.IntegrationEvents.Events.Account.EmailConfirmation;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyResults;

namespace ID.Email.Base.Tests.EventListeners.EmailConfirmation;

public class EmailConfirmationConsumerTests
{
    private readonly Mock<IEmailDetailsTemplateGenerator> _mockTemplateGenerator;
    private readonly Mock<IIdEmailService> _mockEmailService;
    private readonly Mock<ILogger<EmailConfirmationConsumer>> _mockLogger;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions;
    private readonly Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>> _mockCustomerOptions;
    private readonly IdGlobalOptions _globalOptions;
    private readonly IdGlobalSetupOptions_CUSTOMER _customerOptions;
    private readonly EmailConfirmationConsumer _consumer;   
    
    
     //------------------------------------//

    public EmailConfirmationConsumerTests()
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
        _mockLogger = new Mock<ILogger<EmailConfirmationConsumer>>();
        _mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        _mockCustomerOptions = new Mock<IOptions<IdGlobalSetupOptions_CUSTOMER>>();

        // Configure mock options
        _mockGlobalOptions.Setup(x => x.Value).Returns(_globalOptions);
        _mockCustomerOptions.Setup(x => x.Value).Returns(_customerOptions);

        // Create consumer instance
        _consumer = new EmailConfirmationConsumer(
            _mockTemplateGenerator.Object,
            _mockEmailService.Object,
            _mockLogger.Object,
            _mockGlobalOptions.Object,
            _mockCustomerOptions.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenIsCustomerTeamTrue_ShouldCallSendRegistrationEmailCustomer()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
                eventData.Name, eventData.Email, It.IsAny<string>()))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
            eventData.Name, eventData.Email, It.IsAny<string>()), Times.Once);
        
        _mockEmailService.Verify(x => x.SendEmailAsync(expectedEmailDetails), Times.Once);
        
        // Verify that maintenance team method was not called
        _mockTemplateGenerator.Verify(x => x.GenerateEmailConfirmationMntcTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenIsCustomerTeamFalse_ShouldCallSendRegistrationEmailMntc()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: false);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationMntcTemplateAsync(
                eventData.Name, eventData.Email, It.IsAny<string>()))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(successResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert
        _mockTemplateGenerator.Verify(x => x.GenerateEmailConfirmationMntcTemplateAsync(
            eventData.Name, eventData.Email, It.IsAny<string>()), Times.Once);
        
        _mockEmailService.Verify(x => x.SendEmailAsync(expectedEmailDetails), Times.Once);
        
        // Verify that customer team method was not called
        _mockTemplateGenerator.Verify(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleEventAsync_WhenResultNotSucceeded_ShouldLogFailure()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var failureResult = BasicResult.Failure("Email sending failed");

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<EmailDetails>()))
            .ReturnsAsync(failureResult);

        // Act
        await _consumer.HandleEventAsync(eventData);

        // Assert

        //_mockLogger.Verify(x => x.LogBasicResultFailure(
        //    failureResult, IdErrorEvents.Email.EmailConfirmation, "UNKNOWN_EVENT"), Times.Once);
        ExceptionUtils.VerifyBasicResultLogging(
            _mockLogger, IdErrorEvents.Email.EmailConfirmation, failureResult);

    }

    //------------------------------------//


    [Fact]
    public async Task HandleEventAsync_WhenExceptionThrown_ShouldLogException()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedException = new InvalidOperationException("Test exception");

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        // Act
        await _consumer.HandleEventAsync(eventData);       
        
        // Assert
        ExceptionUtils.VerifyExceptionLogging(
            _mockLogger, IdErrorEvents.Email.EmailConfirmation, expectedException);

    }


    //------------------------------------//


    [Fact]
    public async Task SendRegistrationEmailCustomer_ShouldPassCorrectParametersToTemplateGenerator()
    {     
           // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: true);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_customerOptions.CustomerAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmail)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

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
    {        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: false);
        var expectedEmailDetails = CreateTestEmailDetails();
        var successResult = BasicResult.Success();

        var expectedUrl = $"{UrlBuilder.Combine(_globalOptions.MntcAccountsUrl, IdGlobalConstants.EmailRoutes.ConfirmEmail)}?{IdGlobalConstants.EmailRoutes.Params.UserId}={eventData.UserId}&{IdGlobalConstants.EmailRoutes.Params.ConfirmationToken}={eventData.ConfirmationToken}";

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
        var failureResult = BasicResult.Failure("Email service failed");

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationCustomerTemplateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _consumer.SendRegistrationEmailCustomer(eventData);

        // Assert
        result.ShouldBe(failureResult);
        result.Succeeded.ShouldBeFalse();
    }


    //------------------------------------//


    [Fact]
    public async Task SendRegistrationEmailMntc_WhenEmailServiceFails_ShouldReturnFailureResult()
    {
        // Arrange
        var eventData = CreateTestEventData(isCustomerTeam: false);
        var expectedEmailDetails = CreateTestEmailDetails();
        var failureResult = BasicResult.Failure("Email service failed");

        _mockTemplateGenerator
            .Setup(x => x.GenerateEmailConfirmationMntcTemplateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedEmailDetails);

        _mockEmailService
            .Setup(x => x.SendEmailAsync(expectedEmailDetails))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _consumer.SendRegistrationEmailMntc(eventData);

        // Assert
        result.ShouldBe(failureResult);
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//

    private static EmailConfirmationIntegrationEvent CreateTestEventData(bool isCustomerTeam = true)
    {
        return new EmailConfirmationIntegrationEvent(
            userId: Guid.NewGuid(),
            email: "test@example.com",
            phone: "+1234567890",
            name: "Test User",
            confirmationToken: "test-confirmation-token",
            isCustomerTeam: isCustomerTeam);
    }  
    
    
    private static EmailDetails CreateTestEmailDetails()
    {
        return new EmailDetails(
            type: EmailType.HTML,
            message: "<html><body>Test email</body></html>",
            subject: "Email Confirmation",
            toAddress: "test@example.com",
            bccAddresses: [],
            fromAddress: "noreply@example.com",
            fromName: "Test System"
        );
    }


}//Cls
