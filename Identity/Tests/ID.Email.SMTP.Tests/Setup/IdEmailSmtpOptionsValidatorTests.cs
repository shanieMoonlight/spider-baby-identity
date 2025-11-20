namespace ID.Email.SMTP.Tests.Setup;

public class IdEmailSmtpOptionsValidatorTests
{
    [Fact]
    public void Validate_ReturnsSuccess_ForValidOptions()
    {
        // Arrange
        var opts = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        var validator = new IdEmailSmtpOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldBe(ValidateOptionsResult.Success);
    }

    //--------------------//

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ReturnsFailure_WhenSmtpServerAddressMissing(string? server)
    {
        // Arrange
        var opts = new IdEmailSmtpOptions
        {
            SmtpServerAddress = server,
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        var validator = new IdEmailSmtpOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldNotBe(ValidateOptionsResult.Success);
    }

    //--------------------//

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ReturnsFailure_WhenPortInvalid(int port)
    {
        // Arrange
        var opts = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = port,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = "password123"
        };

        var validator = new IdEmailSmtpOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldNotBe(ValidateOptionsResult.Success);
    }

    //--------------------//

    [Fact]
    public void Validate_ReturnsFailure_WhenUsernameMissing()
    {
        // Arrange
        var opts = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = null,
            SmtpPassword = "password123"
        };

        var validator = new IdEmailSmtpOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldNotBe(ValidateOptionsResult.Success);
    }

    //--------------------//

    [Fact]
    public void Validate_ReturnsFailure_WhenPasswordMissing()
    {
        // Arrange
        var opts = new IdEmailSmtpOptions
        {
            SmtpServerAddress = "smtp.example.com",
            SmtpPortNumber = 587,
            SmtpUsernameOrEmail = "user@example.com",
            SmtpPassword = null
        };

        var validator = new IdEmailSmtpOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldNotBe(ValidateOptionsResult.Success);
    }
}
