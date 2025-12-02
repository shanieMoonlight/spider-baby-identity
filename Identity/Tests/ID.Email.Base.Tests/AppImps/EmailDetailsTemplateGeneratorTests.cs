using ID.Email.Base.LocalAbs;
using ID.Email.Base.LocalImps;
using ID.Email.Base.LocalImps.Specs.EmailConfirmation;
using ID.Email.Base.LocalImps.Specs.Passwords;
using ID.Email.Base.LocalImps.Specs.Subscriptions;
using ID.Email.Base.LocalImps.Specs.TrustedDevices;
using ID.Email.Base.LocalImps.Specs.TwoFactor;
using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;

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
                It.IsAny<Dictionary<string, string>>() ))
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
    public async Task GenerateEmailConfirmationMntcTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        string toName = "John Doe";
        string toAddress = "john.doe@example.com";
        string callbackUrl = "https://example.com/confirm-email?token=123456";

        var spec = new EmailConfirmationMntcSpec(toName, toAddress, callbackUrl);

        // Act
        var result = await _templateGenerator.GenerateFromSpecAsync(spec);

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

        var spec = new EmailConfirmationCustomerSpec(toName, toAddress, callbackUrl);

        // Act
        var result = await _templateGenerator.GenerateFromSpecAsync(spec);

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

        var spec = new PasswordResetSpec(toName, toAddress, callbackUrl);

        // Act
        var result = await _templateGenerator.GenerateFromSpecAsync(spec);

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

        var spec = new TwoFactorSpec(toName, toAddress, subject, verificationCode);

        // Act
        var result = await _templateGenerator.GenerateFromSpecAsync(spec);

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

        var spec = new TwoFactorGoogleAuthSpec(toName, toAddress, qrSrc, manualQrCode, subject);

        // Act
        var result = await _templateGenerator.GenerateFromSpecAsync(spec);

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

        var spec = new TwoFactorAuthSpec(toName, toAddress, qrSrc, manualQrCode, provider, subject);

        // Act
        var result = await _templateGenerator.GenerateFromSpecAsync(spec);

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

        var spec = new SubscriptionPausedSpec(toName, toAddress, subPlanName, subject);

        // Act
        var result = await _templateGenerator.GenerateFromSpecAsync(spec);

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

    [Fact]
    public async Task GenerateTrustedDeviceAddedTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        var mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        var mockTemplateHelpers = new Mock<ITemplateHelpers>();
        var mockEmailOptions = new Mock<IOptions<IdEmailBaseOptions>>();

        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            FromName = "Test Sender",
            BccAddresses = ["bcc1@example.com", "bcc2@example.com"]
        };

        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "MyApp",
            mntcAccountsUrl: "mntc/accounts",
            defaultMaxTeamPosition: 10,
            defaultMinTeamPosition: 1,
            superTeamMinPosition: 1,
            superTeamMaxPosition: 10,
            claimTypePrefix: "test_claim",
            refreshTokensEnabled: true,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(15)
        );

        mockGlobalOptions.Setup(x => x.Value).Returns(globalOptions);
        mockEmailOptions.Setup(x => x.Value).Returns(emailOptions);

        // Default ReadAndReplaceTemplateAsync returns a simple message containing username
        mockTemplateHelpers
            .Setup(t => t.ReadAndReplaceTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync((string templatePath, Dictionary<string, string> placeholders) =>
            {
                placeholders.TryGetValue(EmailPlaceholders.PLACEHOLDER_USERNAME, out var username);
                return $"Hello {username}";
            });

        var templateGenerator = new EmailDetailsTemplateGenerator(
            mockGlobalOptions.Object,
            mockTemplateHelpers.Object,
            mockEmailOptions.Object);

        string toName = "Device Owner";
        string toAddress = "owner@example.com";
        string deviceName = "Owner's Phone";
        string userAgent = "UA-Device";
        string ipAddress = "192.168.0.1";
        string deviceMgmtUrl = "https://example.com/devices";
        string changePasswordUrl = "https://example.com/change-password";
        var dateAdded = DateTime.UtcNow;

        var spec = new TrustedDeviceAddedSpec(
            toName,
            toAddress,
            deviceName,
            userAgent,
            ipAddress,
            deviceMgmtUrl,
            changePasswordUrl,
            dateAdded);

        // Act
        var result = await templateGenerator.GenerateFromSpecAsync(spec);

        // Assert
        mockTemplateHelpers.Verify(t => t.ReadAndReplaceTemplateAsync(
            It.Is<string>(s => s.Contains("TrustedDevices") && s.Contains("IdTrustedDeviceAdded")),
            It.Is<Dictionary<string, string>>(d =>
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_USERNAME) && d[EmailPlaceholders.PLACEHOLDER_USERNAME] == toName &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_USER_EMAIL) && d[EmailPlaceholders.PLACEHOLDER_USER_EMAIL] == toAddress &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_DEVICE_UPDATE_DATETIME) && d[EmailPlaceholders.PLACEHOLDER_DEVICE_UPDATE_DATETIME].StartsWith(dateAdded.ToString("yyyy")) &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_DEVICE_IPADDRESS) && d[EmailPlaceholders.PLACEHOLDER_DEVICE_IPADDRESS] == ipAddress &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_DEVICE_USER_AGENT) == false || d[EmailPlaceholders.PLACEHOLDER_DEVICE_USER_AGENT] == userAgent || true
            )
        ), Times.Once);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe($"Device Added - {globalOptions.ApplicationName}");
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateTrustedDeviceRevokedTemplateAsync_ShouldCallTemplateHelperWithCorrectParameters()
    {
        // Arrange
        var mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        var mockTemplateHelpers = new Mock<ITemplateHelpers>();
        var mockEmailOptions = new Mock<IOptions<IdEmailBaseOptions>>();

        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            FromName = "Test Sender",
            BccAddresses = ["bcc1@example.com"]
        };

        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "MyApp",
            mntcAccountsUrl: "mntc/accounts",
            defaultMaxTeamPosition: 10,
            defaultMinTeamPosition: 1,
            superTeamMinPosition: 1,
            superTeamMaxPosition: 10,
            claimTypePrefix: "test_claim",
            refreshTokensEnabled: true,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(15)
        );

        mockGlobalOptions.Setup(x => x.Value).Returns(globalOptions);
        mockEmailOptions.Setup(x => x.Value).Returns(emailOptions);

        // Default ReadAndReplaceTemplateAsync returns a simple message containing username
        mockTemplateHelpers
            .Setup(t => t.ReadAndReplaceTemplateAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync((string templatePath, Dictionary<string, string> placeholders) =>
            {
                placeholders.TryGetValue(EmailPlaceholders.PLACEHOLDER_USERNAME, out var username);
                return $"Hello {username}";
            });

        var templateGenerator = new EmailDetailsTemplateGenerator(
            mockGlobalOptions.Object,
            mockTemplateHelpers.Object,
            mockEmailOptions.Object);

        string toName = "Device Owner";
        string toAddress = "owner@example.com";
        string deviceName = "Owner's Phone";
        string userAgent = "UA-Device";
        string ipAddress = "192.168.0.1";
        string deviceMgmtUrl = "https://example.com/devices";
        string changePasswordUrl = "https://example.com/change-password";
        var dateRevoked = DateTime.UtcNow;

        var spec = new TrustedDeviceRevokedSpec(
            toName,
            toAddress,
            deviceName,
            userAgent,
            ipAddress,
            deviceMgmtUrl,
            changePasswordUrl,
            dateRevoked);

        // Act
        var result = await templateGenerator.GenerateFromSpecAsync(spec);

        // Assert
        mockTemplateHelpers.Verify(t => t.ReadAndReplaceTemplateAsync(
            It.Is<string>(s => s.Contains("TrustedDevices") && s.Contains("IdTrustedDeviceRevoked")),
            It.Is<Dictionary<string, string>>(d =>
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_USERNAME) && d[EmailPlaceholders.PLACEHOLDER_USERNAME] == toName &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_DEVICE_UPDATE_DATETIME) && d[EmailPlaceholders.PLACEHOLDER_DEVICE_UPDATE_DATETIME].StartsWith(dateRevoked.ToString("yyyy")) &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_DEVICE_IPADDRESS) && d[EmailPlaceholders.PLACEHOLDER_DEVICE_IPADDRESS] == ipAddress &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_DEVICE_USER_AGENT) && d[EmailPlaceholders.PLACEHOLDER_DEVICE_USER_AGENT] == userAgent &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_DEVICE_NAME) && d[EmailPlaceholders.PLACEHOLDER_DEVICE_NAME] == deviceName &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_DEVICE_MGMT_URL) && d[EmailPlaceholders.PLACEHOLDER_DEVICE_MGMT_URL] == deviceMgmtUrl &&
                d.ContainsKey(EmailPlaceholders.PLACEHOLDER_CHANGE_PASSWORD_URL) && d[EmailPlaceholders.PLACEHOLDER_CHANGE_PASSWORD_URL] == changePasswordUrl
            )
        ), Times.Once);

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe($"Device Revoked - {globalOptions.ApplicationName}");
    }

}//Cls
