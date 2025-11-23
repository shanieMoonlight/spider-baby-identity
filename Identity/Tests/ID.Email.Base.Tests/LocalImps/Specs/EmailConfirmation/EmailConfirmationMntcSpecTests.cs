using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using ID.Email.Base.LocalImps.Specs.EmailConfirmation;
using ID.Email.Base.LocalAbs;
using ID.Email.Base.Setup;
using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;
using Xunit;

namespace ID.Email.Base.Tests.LocalImps.Specs.EmailConfirmation;

public class EmailConfirmationMntcSpecTests
{
    [Fact]
    public async Task BuildAsync_ShouldCallGenerateTemplateWithCallback_AndReturnEmailDetails()
    {
        // Arrange
        var toName = "Employee";
        var toAddress = "employee@example.com";
        var callbackUrl = "https://example.com/confirm?token=emp";

        var spec = new EmailConfirmationMntcSpec(toName, toAddress, callbackUrl);

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

        var helpersMock = new Mock<ITemplateHelpers>();

        helpersMock
            .Setup(h => h.GenerateTemplateWithCallback(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync((string n, string a, string cb, string path, string subject) =>
            {
                return new EmailDetails(
                    EmailType.HTML,
                    $"Hello {n}, confirm at {cb}",
                    subject,
                    [a],
                    emailOptions.BccAddresses,
                    emailOptions.FromAddress,
                    emailOptions.FromName
                );
            });

        // Act
        var result = await spec.BuildAsync(globalOptions, helpersMock.Object, emailOptions);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEmailDetails>();
        result.Type.ShouldBe(EmailType.HTML);
        result.Subject.ShouldBe($"New User - {globalOptions.ApplicationName}");
        result.Message.ShouldContain(callbackUrl);
        result.ToAddresses.ShouldContain(toAddress);
        result.FromAddress.ShouldBe(emailOptions.FromAddress);
        result.FromName.ShouldBe(emailOptions.FromName);

        helpersMock.Verify(h => h.GenerateTemplateWithCallback(
            toName,
            toAddress,
            callbackUrl,
            It.Is<string>(s => s.EndsWith("EmailConfirmation\\IdEmailConfirmationEmployee.html")),
            It.Is<string>(s => s.Contains("New User") && s.Contains(globalOptions.ApplicationName))), Times.Once);
    }
}
