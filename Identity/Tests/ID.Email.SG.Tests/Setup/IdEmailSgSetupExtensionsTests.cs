using ID.Email.SG.Setup;
using ID.Tests.Utility.LibrarySetup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace ID.Email.SG.Tests.Setup;

public class IdEmailSgSetupExtensionsTests
{

    private static IConfigurationRoot GetEmailConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SendGrid:ApiKey"] = "SG.config-api-key-xyz",
                // Base email options required by ID.Email.Base
                ["SendGrid:FromAddress"] = "test@example.com",
                ["SendGrid:FromName"] = "Test Application",
                ["SendGrid:LogoUrl"] = "https://example.com/logo.png",
                ["SendGrid:ToAddresses:0"] = "to1@example.com",
                ["SendGrid:ToAddresses:1"] = "to2@example.com",
                ["SendGrid:CcAddresses:0"] = "cc@example.com",
                ["SendGrid:BccAddresses:0"] = "bcc1@example.com",
                ["SendGrid:BccAddresses:1"] = "bcc2@example.com",
                ["SendGrid:ColorHexBrand"] = "#123456"
            })
            .Build();
    }

    //--------------------//

    [Fact]
    public void AddMyIdEmailSG_FromConfiguration_BindsOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetEmailConfig();

        // Act
        services.AddMyIdEmailSG(configuration, "SendGrid");

        // Assert
        var sp = services.BuildServiceProvider();
        var opts = sp.GetRequiredService<IOptions<IdEmailSgOptions>>().Value;
        opts.ApiKey.ShouldBe("SG.config-api-key-xyz");
    }

    //--------------------//

    [Fact]
    public void AddMyIdEmailSG_ActionConfig_BindsOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMyIdEmailSG(opts => {
            opts.ApiKey = "SG.action-key-abc";

            //For base library options 
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
        var options = sp.GetRequiredService<IOptions<IdEmailSgOptions>>().Value;
        options.ApiKey.ShouldBe("SG.action-key-abc");
    }


    //--------------------//

    [Fact]
    public void AddEmailSgOptionsAction_RegistersImplementationsForAllLocalInterfaces()
    {
        // Scan assembly that contains the setup types and call the registration lambda
        ServiceCollectionReflectionHelper.AssertAllInterfacesRegistered(
            assembly: IdEmailSgAssemblyReference.Assembly,
            namespacePrefix: "ID.Email.SG",
            registerAction: sc => sc.AddMyIdEmailSG(opts =>
            {
                opts.ApiKey = "SG.action-key-abc";

                //For base library options 
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
    public void AddEmailSgOptionsConfiguration_RegistersImplementationsForAllLocalInterfaces()
    {
        var configuration = GetEmailConfig();

        // Scan assembly that contains the setup types and call the registration lambda
        ServiceCollectionReflectionHelper.AssertAllInterfacesRegistered(
            assembly: IdEmailSgAssemblyReference.Assembly,
            namespacePrefix: "ID.Email.SG",
            registerAction: sc => sc.AddMyIdEmailSG(configuration, "SendGrid")
        );
    }
}//Cls
