using ID.Application.AppAbs.Messaging;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppImps.TwoFactor;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.GlobalSettings.Setup.Options;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.TwoFactor;
using ID.Tests.Data.Factories;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Options;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.ApplicationImps.TwoFactor;

public class TwoFactorMsgServiceTests
{
    private readonly Mock<IIdSmsService> _smsServiceMock;
    private readonly Mock<IEventBus> _busMock;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _2FactorServiceMock;
    private readonly TwoFactorMsgService _twoFactorMsgService;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions;

    //- - - - - - - - - - - - - - - - - - - -//

    public TwoFactorMsgServiceTests()
    {
        _mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        _mockGlobalOptions.Setup(x => x.Value).Returns(GlobalOptionsUtils.ValidOptions);

        _smsServiceMock = new Mock<IIdSmsService>();
        _busMock = new Mock<IEventBus>();
        _2FactorServiceMock = new Mock<ITwoFactorVerificationService<AppUser>>();
        _twoFactorMsgService = new TwoFactorMsgService(
            _smsServiceMock.Object,
            _busMock.Object, 
            _2FactorServiceMock.Object,
            _mockGlobalOptions.Object);
    }

    //---------------------------------------//

    [Fact]
    public async Task SendOTPFor2FactorAuth_ShouldReturnSuccess_WhenSmsProviderIsUsed()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = TeamDataFactory.Create(id: teamId);
        var user = AppUserDataFactory.Create(
            teamId: teamId,
            twoFactorProvider: TwoFactorProvider.Sms,
            phoneNumber: "1234567890");
        _2FactorServiceMock.Setup(x => x.GetFirstValidTwoFactorProviderAsync(user)).ReturnsAsync("Sms");
        _2FactorServiceMock.Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<Team>(), user, "Sms")).ReturnsAsync("123456");
        _smsServiceMock.Setup(x => x.SendMsgAsync(user.PhoneNumber!, It.IsAny<string>())).ReturnsAsync(BasicResult.Success());

        // Act
        var result = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value?.TwoFactorProvider.ShouldBe(TwoFactorProvider.Sms);
    }

    //---------------------------------------//

    [Fact]
    public async Task SendOTPFor2FactorAuth_ShouldFallbackToEmail_WhenSmsFails()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = TeamDataFactory.Create(id: teamId);
        var user = AppUserDataFactory.Create(
            teamId: teamId,
            twoFactorProvider: TwoFactorProvider.Sms,
            phoneNumber: "1234567890",
             email: "test@example.com");
        _2FactorServiceMock.Setup(x => x.GetFirstValidTwoFactorProviderAsync(user)).ReturnsAsync("Sms");
        _2FactorServiceMock.Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<Team>(), user, "Sms")).ReturnsAsync("123456");
        _smsServiceMock.Setup(x => x.SendMsgAsync(user.PhoneNumber!, It.IsAny<string>())).ReturnsAsync(BasicResult.Failure("Sms failed"));
        _2FactorServiceMock.Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<Team>(), user, "Email")).ReturnsAsync("654321");
        _busMock.Setup(x => x.Publish(It.IsAny<TwoFactorEmailRequestIntegrationEvent>(), default)).Returns(Task.CompletedTask);

        // Act
        var result = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);

        // Assert
        result.Succeeded.ShouldBeTrue();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        result.Value.TwoFactorProvider.ShouldBe(TwoFactorProvider.Email);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    //---------------------------------------//

    [Fact]
    public async Task SendOTPFor2FactorAuth_ProviderParamOverridesUserProivider()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = TeamDataFactory.Create(id: teamId);
        var user = AppUserDataFactory.Create(
            teamId: teamId,
            twoFactorProvider: TwoFactorProvider.Sms,
            phoneNumber: "1234567890",
             email: "test@example.com");
        _2FactorServiceMock.Setup(x => x.GetFirstValidTwoFactorProviderAsync(user)).ReturnsAsync("Sms");
        _2FactorServiceMock.Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<Team>(), user, "Sms")).ReturnsAsync("123456");
        _smsServiceMock.Setup(x => x.SendMsgAsync(user.PhoneNumber!, It.IsAny<string>())).ReturnsAsync(BasicResult.Failure("Sms failed"));
        _2FactorServiceMock.Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<Team>(), user, "Email")).ReturnsAsync("654321");
        _busMock.Setup(x => x.Publish(It.IsAny<TwoFactorEmailRequestIntegrationEvent>(), default)).Returns(Task.CompletedTask);

        // Act
        var result = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user, TwoFactorProvider.AuthenticatorApp);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value?.TwoFactorProvider.ShouldBe(TwoFactorProvider.AuthenticatorApp);
    }

    //---------------------------------------//

}