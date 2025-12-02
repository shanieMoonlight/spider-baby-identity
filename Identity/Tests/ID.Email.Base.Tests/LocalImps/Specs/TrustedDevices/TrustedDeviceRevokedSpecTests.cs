using ID.Email.Base.LocalAbs;
using ID.Email.Base.LocalImps.Specs.TrustedDevices;
using ID.Tests.Data.GlobalOptions;

namespace ID.Email.Base.Tests.LocalImps.Specs.TrustedDevices;

public class TrustedDeviceRevokedSpecTests
{
    [Fact]
    public async Task BuildAsync_ShouldReturnEmailDetails_WithCorrectProperties()
    {
        // Arrange
        string templatePathEnding = @"TrustedDevices\IdTrustedDeviceRevoked.html";
        var toName = "Alice";
        var toAddress = "alice@example.com";
        var deviceName = "Alice Phone";
        var userAgent = "UA-1";
        var ipAddress = "127.0.0.1";
        var deviceMgmtUrl = "https://example.com/devices";
        var changePasswordUrl = "https://example.com/change-password";
        var dateRevoked = new DateTime(2023, 1, 2, 15, 30, 0);

        var spec = new TrustedDeviceRevokedSpec(
            toName,
            toAddress,
            deviceName,
            userAgent,
            ipAddress,
            deviceMgmtUrl,
            changePasswordUrl,
            dateRevoked);

        // Setup global options
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

        // Setup email options
        var emailOptions = new IdEmailBaseOptions
        {
            FromAddress = "no-reply@example.com",
            FromName = "MyApp",
            BccAddresses = ["bcc1@example.com", " "]
        };

        // Mock template helpers
        var templateHelpersMock = new Mock<ITemplateHelpers>();
        templateHelpersMock
            .Setup(t => t.ReadAndReplaceTemplateAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync((string path, Dictionary<string, string> placeholders) =>
            {
                // simple message that includes some placeholders values
                placeholders.TryGetValue("username", out var username);
                placeholders.TryGetValue("device_name", out var dname);
                placeholders.TryGetValue("device_update_datetime", out var dt);
                return $"Revoked:{username}:{dname}:{dt}";
            });

        // Act
        var result = await spec.BuildAsync(globalOptions, templateHelpersMock.Object, emailOptions);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe($"Device Revoked - {globalOptions.ApplicationName}");
        //result.Message.ShouldStartWith("Revoked:"); Don't the message content change often. It relies on  ReadAndReplaceTemplateAsync
        result.ToAddresses.ShouldContain(toAddress);
        result.FromAddress.ShouldBe(emailOptions.FromAddress);
        result.FromName.ShouldBe(emailOptions.FromName);
        // BccAddresses should include only the non-empty one
        result.BccAddresses.ShouldContain("bcc1@example.com");
        result.BccAddresses.ShouldNotContain(" ");
        result.InlineImages.ShouldBeEmpty();
        templateHelpersMock.Verify(x => x.ReadAndReplaceTemplateAsync(It.Is<string>(x => x.EndsWith(templatePathEnding)), It.IsAny<Dictionary<string, string>>()), Times.Once);


    }
}
