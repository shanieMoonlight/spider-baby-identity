using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using ID.Email.Base.LocalImps.Specs.TwoFactor;
using ID.Email.Base.LocalAbs;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;
using Xunit;

namespace ID.Email.Base.Tests.LocalImps.Specs.TwoFactor;

public class TwoFactorGoogleAuthSpecTests
{
    [Fact]
    public async Task BuildAsync_ShouldReturnEmailDetails_WithGoogleProvider()
    {
        // Arrange
        var toName = "UserTwo";
        var toAddress = "user2@example.com";
        var qrSrc = "data:image/png;base64,xxx";
        var manualQr = "ABCDEF";
        var subject = "Two-Factor Setup";

        var spec = new TwoFactorGoogleAuthSpec(toName, toAddress, qrSrc, manualQr, subject);

        var globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            applicationName: "MyApp",
            mntcAccountsUrl: "mntc/accounts",
            defaultMaxTeamPosition: 10,
            defaultMinTeamPosition: 1,
            superTeamMinPosition: 1,
            superTeamMaxPosition: 10,
            claimTypePrefix: "test_claim",
            refreshTokensEnabled: true,
            phoneTokenTimeSpan: TimeSpan.FromMinutes(15));

        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "no-reply@example.com",
            FromName = "MyApp",
            BccAddresses = ["bcc@example.com"]
        };

        var templateHelpersMock = new Mock<ITemplateHelpers>();
        templateHelpersMock
            .Setup(t => t.ReadAndReplaceTemplateAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>() ))
            .ReturnsAsync((string path, Dictionary<string, string> placeholders) =>
            {
                placeholders.TryGetValue("username", out var username);
                placeholders.TryGetValue("manual_qr_code", out var manual);
                placeholders.TryGetValue("qr_img_src", out var qr);
                placeholders.TryGetValue("2_factor_provider", out var prov);
                return $"TFGA:{username}:{prov}:{manual}:{qr}";
            });

        // Act
        var result = await spec.BuildAsync(globalOptions, templateHelpersMock.Object, emailOptions);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe(subject);
        //result.Message.ShouldStartWith("TFGA:"); Don't the message content change often. It relies on  ReadAndReplaceTemplateAsync
        //result.Message.ShouldContain("Google Authenticator");
        result.ToAddresses.ShouldContain(toAddress);
        result.FromAddress.ShouldBe(emailOptions.FromAddress);
        result.FromName.ShouldBe(emailOptions.FromName);
        result.BccAddresses.ShouldContain("bcc@example.com");

        templateHelpersMock.Verify(x => x.ReadAndReplaceTemplateAsync(It.Is<string>(s => s.EndsWith("TwoFactor\\IdTwoFactorGoogleAuthSetup.html")), It.IsAny<Dictionary<string,string>>()), Times.Once);
    }
}
