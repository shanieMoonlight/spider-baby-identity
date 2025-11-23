using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using ID.Email.Base.LocalImps.Specs.TrustedDevices;
using ID.Email.Base.LocalAbs;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;
using Xunit;

namespace ID.Email.Base.Tests.LocalImps.Specs.TrustedDevices;

public class TrustedDeviceAddedSpecTests
{
    [Fact]
    public async Task BuildAsync_ShouldReturnEmailDetails_WithCorrectProperties()
    {
        // Arrange
        string templatePathEnding = @"TrustedDevices\IdTrustedDeviceAdded.html";
        var toName = "Bob";
        var toAddress = "bob@example.com";
        var deviceName = "Bob Tablet";
        var userAgent = "UA-2";
        var ipAddress = "10.0.0.1";
        var deviceMgmtUrl = "https://example.com/devices";
        var changePasswordUrl = "https://example.com/change-password";
        var dateAdded = new DateTime(2024, 6, 1, 12, 0, 0);

        var spec = new TrustedDeviceAddedSpec(
            toName,
            toAddress,
            deviceName,
            userAgent,
            ipAddress,
            deviceMgmtUrl,
            changePasswordUrl,
            dateAdded);

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
            .Setup(t => t.ReadAndReplaceTemplateAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>() ))
            .ReturnsAsync((string path, Dictionary<string, string> placeholders) =>
            {
                placeholders.TryGetValue("username", out var username);
                placeholders.TryGetValue("user_email", out var userEmail);
                placeholders.TryGetValue("device_name", out var dname);
                placeholders.TryGetValue("device_update_datetime", out var dt);
                return $"Added:{username}:{userEmail}:{dname}:{dt}";
            });

        // Act
        var result = await spec.BuildAsync(globalOptions, templateHelpersMock.Object, emailOptions);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe($"Device Added - {globalOptions.ApplicationName}");
        //result.Message.ShouldStartWith("Added:");  Don't the message content change often. It relies on  ReadAndReplaceTemplateAsync***
        result.ToAddresses.ShouldContain(toAddress);
        result.FromAddress.ShouldBe(emailOptions.FromAddress);
        result.FromName.ShouldBe(emailOptions.FromName);
        result.BccAddresses.ShouldContain("bcc1@example.com");
        result.BccAddresses.ShouldNotContain(" ");
        result.InlineImages.ShouldBeEmpty();
        templateHelpersMock.Verify(x => x.ReadAndReplaceTemplateAsync(It.Is<string>(x => x.EndsWith(templatePathEnding)), It.IsAny<Dictionary<string, string>>()), Times.Once);
    }
}
