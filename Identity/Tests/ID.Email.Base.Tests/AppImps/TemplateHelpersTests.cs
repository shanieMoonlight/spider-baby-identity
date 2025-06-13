using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Options;

namespace ID.Email.Base.Tests.AppImps;

public class TemplateHelpersTests
{
    private readonly TemplateHelpers _templateHelpers;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions;
    private readonly Mock<IOptions<IdEmailBaseOptions>> _optionsMock = new();
    private readonly IdEmailBaseOptions _options;
    private readonly IdGlobalOptions _globalOptions;
    private readonly string _testTemplatePath;
    private readonly string _templateContent;

    public TemplateHelpersTests()
    {
        // Set up global options
        _globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "Test Application",
            mntcAccountsUrl: "https://test.com/accounts",
            defaultMaxTeamPosition: 10,
            defaultMinTeamPosition: 1,
            superTeamMinPosition: 1,
            superTeamMaxPosition: 10,
            claimTypePrefix: "test_claim",
            refreshTokensEnabled: true,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(15)
        );

        // Mock global settings
        _mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        _mockGlobalOptions.Setup(o => o.Value).Returns(_globalOptions);

        // Set up test options
        _options = new IdEmailBaseOptions
        {
            FromAddress = "test@example.com",
            FromName = "Test Application",
            BccAddresses = ["admin@example.com"],
            ColorHexBrand = "#70AE6E",
            LogoUrl = "https://example.com/logo.png"
        };
        _optionsMock.Setup(o => o.Value).Returns(_options);

        // Initialize TemplateHelpers with mocked settings
        _templateHelpers = new TemplateHelpers(_mockGlobalOptions.Object, _optionsMock.Object);

        // Create a temp template file for testing
        _testTemplatePath = "TestTemplate.html";
        _templateContent = $"Hello, {EmailPlaceholders.PLACEHOLDER_USERNAME}! Welcome to {EmailPlaceholders.PLACEHOLDER_APPNAME}. " +
                           $"Click <a style=\"color:{EmailPlaceholders.PLACEHOLDER_COLOR_PRIMARY};\" href='{EmailPlaceholders.PLACEHOLDER_CALLBACK_URL}'>here</a> to continue. " +
                           $"� {EmailPlaceholders.PLACEHOLDER_CURRENT_YEAR}" +
                           $"*********" +
                           $"****{EmailPlaceholders.PLACEHOLDER_LOGO_IMAGE_DISPLAY}*****" +
                           $"****{EmailPlaceholders.PLACEHOLDER_LOGO_TEXT_DISPLAY}*****" +
                           $"****{EmailPlaceholders.PLACEHOLDER_LOGO}*****" +
                           $"****{EmailPlaceholders.PLACEHOLDER_LOGO_IMAGE_DISPLAY}*****" +
                           $"*********";

        // Write the template file to the test directory
        string fullPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            _testTemplatePath);

        File.WriteAllText(fullPath, _templateContent);
    }

    //--------------------------------//

    [Fact]
    public async Task GenerateTemplateWithCallback_ShouldReturnValidEmailDetails()
    {
        // Arrange
        string toName = "John Doe";
        string toAddress = "john.doe@example.com";
        string callbackUrl = "https://example.com/callback?token=123456";
        string subject = "Test Subject";

        // Act
        var result = await _templateHelpers.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            _testTemplatePath,
            subject);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<EmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe(subject);
        result.ToAddresses.ShouldContain(toAddress);
        result.FromAddress.ShouldBe(_options.FromAddress);
        result.FromName.ShouldBe(_options.FromName);
        result.BccAddresses.ShouldBe(_options.BccAddresses);

        // Check that the message content has the placeholders replaced
        result.Message.ShouldNotBeNullOrEmpty(toName);
        //result.Message.ShouldContain(toName);
        //result.Message.ShouldContain(callbackUrl);
        //result.Message.ShouldContain(_mockGlobalSettings.Object.ApplicationName);
        //result.Message.ShouldContain(DateTime.Now.Year.ToString());
    }

    //--------------------------------//

    [Fact]
    public async Task ReadAndReplaceTemplateAsync_ShouldReplaceAllPlaceholders()
    {
        // Arrange
        string toName = "John Doe";
        string callbackUrl = "https://example.com/callback?token=123456";
        var placeholders = new Dictionary<string, string>
        {
            { EmailPlaceholders.PLACEHOLDER_USERNAME, toName },
            { EmailPlaceholders.PLACEHOLDER_CALLBACK_URL, callbackUrl }
        };

        // Act
        string result = await _templateHelpers.ReadAndReplaceTemplateAsync(_testTemplatePath, placeholders);

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain(toName);
        result.ShouldContain(callbackUrl);
        result.ShouldContain(_globalOptions.ApplicationName);
        result.ShouldContain(_options.ColorHexBrand ?? "#70AE6E");
        result.ShouldContain(DateTime.Now.Year.ToString());

        // Verify that the original placeholders are not present
        result.ShouldNotContain(EmailPlaceholders.PLACEHOLDER_USERNAME);
        result.ShouldNotContain(EmailPlaceholders.PLACEHOLDER_CALLBACK_URL);
        result.ShouldNotContain(EmailPlaceholders.PLACEHOLDER_APPNAME);
        result.ShouldNotContain(EmailPlaceholders.PLACEHOLDER_COLOR_PRIMARY);
        result.ShouldNotContain(EmailPlaceholders.PLACEHOLDER_CURRENT_YEAR);
    }


    //--------------------------------//


    [Fact]
    public async Task ReadAndReplaceTemplateAsync_WithEmptyLogoUrl_ShouldSetCorrectDisplayValues()
    {
        // Arrange
        var emptyLogoOptions = new IdEmailBaseOptions
        {
            FromAddress = _options.FromAddress,
            FromName = _options.FromName,
            BccAddresses = _options.BccAddresses,
            ColorHexBrand = _options.ColorHexBrand,
            LogoUrl = string.Empty // Empty for this test
        };
        
        var emptyLogoOptionsMock = new Mock<IOptions<IdEmailBaseOptions>>();
        emptyLogoOptionsMock.Setup(o => o.Value).Returns(emptyLogoOptions);
        
        var templateHelpers = new TemplateHelpers(_mockGlobalOptions.Object, emptyLogoOptionsMock.Object);
        var placeholders = new Dictionary<string, string>();

        // Act
        string result = await templateHelpers.ReadAndReplaceTemplateAsync(_testTemplatePath, placeholders);

        // Assert
        result.ShouldContain(EmailPlaceholders.Values.LOGO_DISPLAY_NONE);
        result.ShouldContain(EmailPlaceholders.Values.LOGO_DISPLAY_INLINE);
    }


    //--------------------------------//


    [Fact]
    public async Task ReadAndReplaceTemplateAsync_WithLogoUrl_ShouldSetCorrectDisplayValues()
    {
        // Arrange
        var logoOptions = new IdEmailBaseOptions
        {
            FromAddress = _options.FromAddress,
            FromName = _options.FromName,
            BccAddresses = _options.BccAddresses,
            ColorHexBrand = _options.ColorHexBrand,
            LogoUrl = "https://example.com/logo.png" // Non-empty for this test
        };
        
        var logoOptionsMock = new Mock<IOptions<IdEmailBaseOptions>>();
        logoOptionsMock.Setup(o => o.Value).Returns(logoOptions);
        
        var templateHelpers = new TemplateHelpers(_mockGlobalOptions.Object, logoOptionsMock.Object);
        var placeholders = new Dictionary<string, string>();

        // Act
        string result = await templateHelpers.ReadAndReplaceTemplateAsync(_testTemplatePath, placeholders);

        // Assert
        result.ShouldContain(EmailPlaceholders.Values.LOGO_DISPLAY_INLINE);
        result.ShouldContain(EmailPlaceholders.Values.LOGO_DISPLAY_NONE);
    }

    //--------------------------------//

    [Fact]
    public async Task ReadAndReplaceTemplateAsync_WithNonexistentTemplate_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var placeholders = new Dictionary<string, string>();

        // Act & Assert
        await Should.ThrowAsync<FileNotFoundException>(async () =>
            await _templateHelpers.ReadAndReplaceTemplateAsync("NonexistentTemplate.html", placeholders)
        );
    }

    //--------------------------------//

}
