using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Options;

namespace ID.Email.Base.Tests.AppImps;

public class EmailDetailsTemplateGeneratorTests
{
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions;
    private readonly Mock<ITemplateHelpers> _mockTemplateHelpers;
    private readonly Mock<IOptions<IdEmailBaseOptions>> _optionsMock;
    private readonly IdEmailBaseOptions _emailOptions;
    private readonly IdGlobalOptions _globalOptions;
    private readonly EmailDetailsTemplateGenerator _templateGenerator;
    private readonly string _applicationName = "Test Application";
    private readonly string _mntcAccountsUrl = "mntc/accounts";

    //- - - - - - - - - - - - - - - - - - //



    public EmailDetailsTemplateGeneratorTests()
    {
        // Setup email options with test data
        _emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            FromName = "Test Sender",
            BccAddresses = ["bcc1@example.com", "bcc2@example.com"],
            LogoUrl = "https://example.com/logo.png",
            ColorHexBrand = "#0096c7"
        };
        
        // Setup global options with test data
        _globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: _applicationName,
            mntcAccountsUrl: _mntcAccountsUrl,
            defaultMaxTeamPosition: 10,
            defaultMinTeamPosition: 1,
            superTeamMinPosition: 1,
            superTeamMaxPosition: 10,
            claimTypePrefix: "test_claim",
            refreshTokensEnabled: true,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(15)
        );

        // Setup mocks
        _mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        _mockTemplateHelpers = new Mock<ITemplateHelpers>();
        _optionsMock = new Mock<IOptions<IdEmailBaseOptions>>();

        // Configure mock options
        _mockGlobalOptions.Setup(x => x.Value).Returns(_globalOptions);
        _optionsMock.Setup(x => x.Value).Returns(_emailOptions);

        // Initialize template generator with mock dependencies
        _templateGenerator = new EmailDetailsTemplateGenerator(
            _mockGlobalOptions.Object,
            _mockTemplateHelpers.Object,
            _optionsMock.Object);

        // Setup default mocked behavior for template helpers
        SetupMockTemplateHelpers();

    }
    
    //------------------------------------//

    private void SetupMockTemplateHelpers()
    {
        // Mock the GenerateTemplateWithCallback method
        _mockTemplateHelpers
            .Setup(t => t.GenerateTemplateWithCallback(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((string toName, string toAddress, string callbackUrl, string templatePath, string subject) =>
            {
                return new EmailDetails(
                    EmailType.HTML,
                    $"Hello, {toName}! Click here: {callbackUrl}",
                    subject,
                    [toAddress],
                    _emailOptions.BccAddresses,
                    _emailOptions.FromAddress ?? "default@test.com",
                    _emailOptions.FromName ?? "Default Sender"
                );
            });

        // Mock the ReadAndReplaceTemplateAsync method
        _mockTemplateHelpers
            .Setup(t => t.ReadAndReplaceTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync((string templatePath, Dictionary<string, string> placeholders) =>
            {
                // Create a simple message with placeholders replaced
                string message = "Template content";

                if (placeholders.TryGetValue(EmailPlaceholders.PLACEHOLDER_USERNAME, out string? username))
                    message = $"Hello, {username}!";

                if (placeholders.TryGetValue(EmailPlaceholders.PLACEHOLDER_VERIFICATION_CODE, out string? code))
                    message += $" Your code is: {code}";

                if (placeholders.TryGetValue(EmailPlaceholders.PLACEHOLDER_MANUAL_QR_CODE, out string? qrCode))
                    message += $" Manual code: {qrCode}";

                if (placeholders.TryGetValue(EmailPlaceholders.PLACEHOLDER_QR_IMG_SRC, out string? qrSrc))
                    message += $" Image src: {qrSrc}";

                if (placeholders.TryGetValue(EmailPlaceholders.PLACEHOLDER_2_FACTOR_PROVIDER, out string? provider))
                    message += $" Provider: {provider}";

                if (placeholders.TryGetValue(EmailPlaceholders.PLACEHOLDER_SUB_PLAN_NAME, out string? planName))
                    message += $" Subscription: {planName}";

                return message;
            });
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateChangePasswordTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        string toName = "John Doe";
        string toAddress = "john.doe@example.com";
        string callbackUrl = "https://example.com/reset-password?token=123456";

        // Act
        var result = await _templateGenerator.GenerateChangePasswordTemplateAsync(toName, toAddress, callbackUrl);

        // Assert
        _mockTemplateHelpers.Verify(t => t.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            It.Is<string>(s => s.Contains("ResetPassword")),
            It.Is<string>(s => s.Contains("New User") && s.Contains(_applicationName))),
            Times.Once);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateEmailConfirmationMntcTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        string toName = "John Doe";
        string toAddress = "john.doe@example.com";
        string callbackUrl = "https://example.com/confirm-email?token=123456";

        // Act
        var result = await _templateGenerator.GenerateEmailConfirmationMntcTemplateAsync(toName, toAddress, callbackUrl);

        // Assert
        _mockTemplateHelpers.Verify(t => t.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            It.Is<string>(s => s.Contains("EmailConfirmationEmployee")),
            It.Is<string>(s => s.Contains("New User") && s.Contains(_applicationName))),
            Times.Once);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateEmailConfirmationCustomerTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        string toName = "John Doe";
        string toAddress = "john.doe@example.com";
        string callbackUrl = "https://example.com/confirm-email?token=123456";

        // Act
        var result = await _templateGenerator.GenerateEmailConfirmationCustomerTemplateAsync(toName, toAddress, callbackUrl);

        // Assert
        _mockTemplateHelpers.Verify(t => t.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            It.Is<string>(s => s.Contains("EmailConfirmationCustomer")),
            It.Is<string>(s => s.Contains("New User") && s.Contains(_applicationName))),
            Times.Once);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
    }

    //------------------------------------//

    [Fact]
    public async Task GeneratePasswordResetTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        string toName = "John Doe";
        string toAddress = "john.doe@example.com";
        string callbackUrl = "https://example.com/reset-password?token=123456";

        // Act
        var result = await _templateGenerator.GeneratePasswordResetTemplateAsync(toName, toAddress, callbackUrl);

        // Assert
        _mockTemplateHelpers.Verify(t => t.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            It.Is<string>(s => s.Contains("ResetPassword")),
            It.Is<string>(s => s.Contains("Password Reset") && s.Contains(_applicationName))),
            Times.Once);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateTwoFactorTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        string toName = "John Doe";
        string toAddress = "john.doe@example.com";
        string subject = "Your verification code";
        string verificationCode = "123456";

        // Act
        var result = await _templateGenerator.GenerateTwoFactorTemplateAsync(toName, toAddress, subject, verificationCode);

        // Assert
        _mockTemplateHelpers.Verify(t => t.ReadAndReplaceTemplateAsync(
            It.Is<string>(s => s.Contains("TwoFactor")),
            It.Is<Dictionary<string, string>>(d =>
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_USERNAME) &&
                d[EmailPlaceholders.PLACEHOLDER_USERNAME] == toName &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_VERIFICATION_CODE) &&
                d[EmailPlaceholders.PLACEHOLDER_VERIFICATION_CODE] == verificationCode)),
            Times.Once);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe(subject);
        result.ToAddresses.ShouldContain(toAddress);
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateTwoFactorGoogleAuthTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        string toName = "John Doe";
        string toAddress = "john.doe@example.com";
        string qrSrc = "data:image/png;base64,abc123";
        string manualQrCode = "ABCDEFGHIJK";
        string subject = "Two-Factor Authentication Setup";

        // Act
        var result = await _templateGenerator.GenerateTwoFactorGoogleAuthTemplateAsync(toName, toAddress, qrSrc, manualQrCode, subject);

        // Assert
        _mockTemplateHelpers.Verify(t => t.ReadAndReplaceTemplateAsync(
            It.Is<string>(s => s.Contains("TwoFactorGoogleAuthSetup")),
            It.Is<Dictionary<string, string>>(d =>
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_USERNAME) &&
                d[EmailPlaceholders.PLACEHOLDER_USERNAME] == toName &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_2_FACTOR_PROVIDER) &&
                d[EmailPlaceholders.PLACEHOLDER_2_FACTOR_PROVIDER] == "Google Authenticator" &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_MANUAL_QR_CODE) &&
                d[EmailPlaceholders.PLACEHOLDER_MANUAL_QR_CODE] == manualQrCode &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_QR_IMG_SRC) &&
                d[EmailPlaceholders.PLACEHOLDER_QR_IMG_SRC] == qrSrc)),
            Times.Once);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe(subject);
        result.ToAddresses.ShouldContain(toAddress);
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateTwoFactorAuthTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        string toName = "John Doe";
        string toAddress = "john.doe@example.com";
        string qrSrc = "data:image/png;base64,abc123";
        string manualQrCode = "ABCDEFGHIJK";
        string provider = "Microsoft Authenticator";
        string subject = "Two-Factor Authentication Setup";

        // Act
        var result = await _templateGenerator.GenerateTwoFactorAuthTemplateAsync(toName, toAddress, qrSrc, manualQrCode, provider, subject);

        // Assert
        _mockTemplateHelpers.Verify(t => t.ReadAndReplaceTemplateAsync(
            It.Is<string>(s => s.Contains("TwoFactorGoogleAuthSetup")),
            It.Is<Dictionary<string, string>>(d =>
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_USERNAME) &&
                d[EmailPlaceholders.PLACEHOLDER_USERNAME] == toName &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_2_FACTOR_PROVIDER) &&
                d[EmailPlaceholders.PLACEHOLDER_2_FACTOR_PROVIDER] == provider &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_MANUAL_QR_CODE) &&
                d[EmailPlaceholders.PLACEHOLDER_MANUAL_QR_CODE] == manualQrCode &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_QR_IMG_SRC) &&
                d[EmailPlaceholders.PLACEHOLDER_QR_IMG_SRC] == qrSrc)),
            Times.Once);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe(subject);
        result.ToAddresses.ShouldContain(toAddress);
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateSubscriptionPausedTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        string toName = "John Doe";
        string toAddress = "john.doe@example.com";
        string subPlanName = "Premium Plan";
        string subject = "Your Subscription Has Been Paused";

        // Act
        var result = await _templateGenerator.GenerateSubscriptionPausedTemplateAsync(toName, toAddress, subPlanName, subject);

        // Assert
        _mockTemplateHelpers.Verify(t => t.ReadAndReplaceTemplateAsync(
            It.Is<string>(s => s.Contains("IdSubPaused")),
            It.Is<Dictionary<string, string>>(d =>
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_USERNAME) &&
                d[EmailPlaceholders.PLACEHOLDER_USERNAME] == toName &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_SUB_PLAN_NAME) &&
                d[EmailPlaceholders.PLACEHOLDER_SUB_PLAN_NAME] == subPlanName)),
            Times.Once);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe(subject);
        result.ToAddresses.ShouldContain(toAddress);
    }

    //------------------------------------//

}//Cls
