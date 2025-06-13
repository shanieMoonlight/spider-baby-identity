using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppImps;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.EmailConfirmation;
using ID.Tests.Data.Factories;
using Moq;

namespace ID.Application.Tests.ApplicationImps;
public class EmailConfirmationBusTests
{

    private readonly Mock<IEmailConfirmationService<AppUser>> _emailConfServiceMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly EmailConfirmationBus _emailConfirmationBus;

    //- - - - - - - - - - - - - - - - - - - - -//

    public EmailConfirmationBusTests()
    {
        _emailConfServiceMock = new Mock<IEmailConfirmationService<AppUser>>();
        _eventBusMock = new Mock<IEventBus>();
        _emailConfirmationBus = new EmailConfirmationBus(_emailConfServiceMock.Object, _eventBusMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateTokenAndPublishEventAsync_ShouldNotPublishEvent_WhenEmailIsAlreadyConfirmed()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team, emailConfirmed: true);
        var cancellationToken = CancellationToken.None;

        // Act
        await _emailConfirmationBus.GenerateTokenAndPublishEventAsync(user, team, cancellationToken);

        // Assert
        _emailConfServiceMock.Verify(x => x.GenerateSafeEmailConfirmationTokenAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Never);
        _eventBusMock.Verify(x => x.Publish(It.IsAny<IIdIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateTokenAndPublishEventAsync_ShouldPublishEventRequiringPassword_WhenPasswordHashIsNullOrWhiteSpace()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team, emailConfirmed: false, passwordHash: "");
        var cancellationToken = CancellationToken.None;
        var token = "safeToken";

        _emailConfServiceMock.Setup(x => x.GenerateSafeEmailConfirmationTokenAsync(team, user)).ReturnsAsync(token);

        // Act
        await _emailConfirmationBus.GenerateTokenAndPublishEventAsync(user, team, cancellationToken);

        // Assert
        _emailConfServiceMock.Verify(x => x.GenerateSafeEmailConfirmationTokenAsync(team, user), Times.Once);
        _eventBusMock.Verify(x => x.Publish(It.Is<EmailConfirmationRequiringPasswordIntegrationEvent>(e => e.UserId == user.Id && e.ConfirmationToken == token), cancellationToken), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task GenerateTokenAndPublishEventAsync_ShouldPublishEvent_WhenPasswordHashIsNotNullOrWhiteSpace()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team, emailConfirmed: false, password: "Pa$$ord");

        var cancellationToken = CancellationToken.None;
        var token = "safeToken";

        _emailConfServiceMock.Setup(x => x.GenerateSafeEmailConfirmationTokenAsync(team, user)).ReturnsAsync(token);

        // Act
        await _emailConfirmationBus.GenerateTokenAndPublishEventAsync(user, team, cancellationToken);

        // Assert
        _emailConfServiceMock.Verify(x => x.GenerateSafeEmailConfirmationTokenAsync(team, user), Times.Once);
        _eventBusMock.Verify(x => x.Publish(It.Is<EmailConfirmationIntegrationEvent>(e => e.UserId == user.Id && e.ConfirmationToken == token), cancellationToken), Times.Once);
    }

    //------------------------------------//




}
