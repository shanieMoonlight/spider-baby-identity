using ID.Tests.Utility.LibrarySetup;
using ID.Email.SMTP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace ID.Email.SMTP.Tests.Setup;

public class IdEmailSmtpSetupTests
{
    private static IConfigurationRoot GetEmailConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Smtp:SmtpServerAddress"] = "smtp.example.com",
            })
            .Build();
    }

    //--------------------//

    [Fact]
    public void AddMyIdEmailSmtp_FromConfiguration_BindsOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Smtp:SmtpServerAddress"] = "smtp.config.example.com",
                ["Smtp:SmtpPortNumber"] = "587",
                ["Smtp:SmtpUsernameOrEmail"] = "user@example.com",
                ["Smtp:SmtpPassword"] = "password",
                // Base email options
                ["Smtp:FromAddress"] = "test@example.com",
                ["Smtp:FromName"] = "Test Application",
                ["Smtp:LogoUrl"] = "https://example.com/logo.png",
                ["Smtp:ToAddresses:0"] = "to1@example.com",
                ["Smtp:ToAddresses:1"] = "to2@example.com",
                ["Smtp:CcAddresses:0"] = "cc@example.com",
                ["Smtp:BccAddresses:0"] = "bcc1@example.com",
                ["Smtp:BccAddresses:1"] = "bcc2@example.com",
                ["Smtp:ColorHexBrand"] = "#123456"
            })
            .Build();

        // Act
        services.AddMyIdEmailSmtp(configuration, "Smtp");

        // Assert
        var sp = services.BuildServiceProvider();
        var opts = sp.GetRequiredService<IOptions<IdEmailSmtpOptions>>().Value;
        opts.SmtpServerAddress.ShouldBe("smtp.config.example.com");
        opts.SmtpPortNumber.ShouldBe(587);
        opts.SmtpUsernameOrEmail.ShouldBe("user@example.com");
        opts.SmtpPassword.ShouldBe("password");
    }

    //--------------------//

    [Fact]
    public void AddMyIdEmailSmtp_ActionConfig_BindsOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdEmailSmtp(opts => {
            opts.SmtpServerAddress = "smtp.action.example.com";
            opts.SmtpPortNumber = 25;
            opts.SmtpUsernameOrEmail = "actionuser@example.com";
            opts.SmtpPassword = "actionpass";

            // For base library options
            opts.FromAddress = "test@example.com";
            opts.FromName = "Test Application";
            opts.LogoUrl = "https://example.com/logo.png";
            opts.ToAddresses = ["to1@example.com", "to2@example.com"];
            opts.CcAddresses = ["cc@example.com"];
            opts.BccAddresses = ["bcc1@example.com", "bcc2@example.com"];
            opts.ColorHexBrand = "#123456";
        });

        // Assert
        var sp = services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<IdEmailSmtpOptions>>().Value;
        options.SmtpServerAddress.ShouldBe("smtp.action.example.com");
        options.SmtpPortNumber.ShouldBe(25);
        options.SmtpUsernameOrEmail.ShouldBe("actionuser@example.com");
        options.SmtpPassword.ShouldBe("actionpass");
    }


    //--------------------//

    [Fact]
    public void AddEmailSmtpOptionsAction_RegistersImplementationsForAllLocalInterfaces()
    {
        // Scan assembly that contains the setup types and call the registration lambda
        ServiceCollectionReflectionHelper.AssertAllInterfacesRegistered(
            assembly: IdEmailSmtpAssemblyReference.Assembly,
            namespacePrefix: "ID.Email.SMTP",
            registerAction: sc => sc.AddMyIdEmailSmtp(opts =>
            {
                opts.SmtpServerAddress = "smtp.action.example.com";
                opts.SmtpPortNumber = 25;
                opts.SmtpUsernameOrEmail = "actionuser@example.com";
                opts.SmtpPassword = "actionpass";

                // For base library options
                opts.FromAddress = "test@example.com";
                opts.FromName = "Test Application";
                opts.LogoUrl = "https://example.com/logo.png";
                opts.ToAddresses = ["to1@example.com", "to2@example.com"];
                opts.CcAddresses = ["cc@example.com"];
                opts.BccAddresses = ["bcc1@example.com", "bcc2@example.com"];
                opts.ColorHexBrand = "#123456";
            })
        );
    }


    //--------------------//

    [Fact]
    public void AddEmailSmtpOptionsConfiguration_RegistersImplementationsForAllLocalInterfaces()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Smtp:SmtpServerAddress"] = "smtp.config.example.com",
                ["Smtp:SmtpPortNumber"] = "587",
                ["Smtp:SmtpUsernameOrEmail"] = "user@example.com",
                ["Smtp:SmtpPassword"] = "password",

                // Base email options
                ["Smtp:FromAddress"] = "test@example.com",
                ["Smtp:FromName"] = "Test Application",
                ["Smtp:LogoUrl"] = "https://example.com/logo.png",
                ["Smtp:ToAddresses:0"] = "to1@example.com",
                ["Smtp:ToAddresses:1"] = "to2@example.com",
                ["Smtp:CcAddresses:0"] = "cc@example.com",
                ["Smtp:BccAddresses:0"] = "bcc1@example.com",
                ["Smtp:BccAddresses:1"] = "bcc2@example.com",
                ["Smtp:ColorHexBrand"] = "#123456"
            })
            .Build();

        // Scan assembly that contains the setup types and call the registration lambda
        ServiceCollectionReflectionHelper.AssertAllInterfacesRegistered(
            assembly: IdEmailSmtpAssemblyReference.Assembly,
            namespacePrefix: "ID.Email.SMTP",
            registerAction: sc => sc.AddMyIdEmailSmtp(configuration, "Smtp")
        );
    }
}
