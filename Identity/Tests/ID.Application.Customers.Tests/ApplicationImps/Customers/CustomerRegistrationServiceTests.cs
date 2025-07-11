using ClArch.ValueObjects;
using ID.Application.Customers.ApplicationImps;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.AppServices.Abs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Customers.Tests.ApplicationImps.Customers;

public class CustomerRegistrationServiceTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly IdCustomerRegistrationService _regService;
    private readonly Mock<ITeamSubscriptionService> _subServiceMock;
    private readonly Mock<ITeamBuilderService> _teamBuilderMock;

    public CustomerRegistrationServiceTests()
    {
        _teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        _subServiceMock = new Mock<ITeamSubscriptionService>();
        _teamBuilderMock = new Mock<ITeamBuilderService>();

        _regService = new IdCustomerRegistrationService(_teamManagerMock.Object, _teamBuilderMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task RegisterAsync_ShouldRegisterCustomerSuccessfully()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id);
        var email = EmailAddress.Create(appUser.Email!);
        var username = UsernameNullable.Create(appUser.UserName);
        var phone = PhoneNullable.Create(appUser.PhoneNumber);
        var firstName = FirstNameNullable.Create(appUser.FirstName);
        var lastName = LastNameNullable.Create(appUser.LastName);
        var password = Password.Create("Password123!");
        var confPassword = ConfirmPassword.Create("Password123!");
        var position = TeamPositionNullable.Create(1);
        Guid subscriptionPlanId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _teamManagerMock.Setup(m => m.AddTeamAsync(It.IsAny<Team>(), cancellationToken))
            .ReturnsAsync(team);
        _teamManagerMock.Setup(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), password.Value))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));
        _teamManagerMock.Setup(m => m.GetSubscriptionServiceAsync(team))
            .ReturnsAsync(GenResult<ITeamSubscriptionService>.Success(_subServiceMock.Object));
        _teamManagerMock.Setup(m => m.UpdateAsync(team))
            .ReturnsAsync(team);

        _subServiceMock.Setup(m => m.Team).Returns(team);
        _subServiceMock.Setup(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()))
            .ReturnsAsync(GenResult<TeamSubscription>.Success(SubscriptionDataFactory.Create()));

        // Act
        var result = await _regService.RegisterAsync(email, username, phone, firstName, lastName, password, confPassword, position, subscriptionPlanId, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Email.ShouldBe(email.Value);
        result.Value.UserName.ShouldBe(username.Value);
        _teamManagerMock.Verify(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), password.Value), Times.Once);
        _subServiceMock.Verify(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()), Times.Once);
        _teamManagerMock.Verify(m => m.UpdateAsync(team), Times.AtLeastOnce);
    }

    //------------------------------------//

    [Fact]
    public async Task RegisterAsync_ShouldRegisterCustomerSuccessfully_WhenNoSupsriptiopnPlanId()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id);
        var email = EmailAddress.Create(appUser.Email!);
        var username = UsernameNullable.Create(appUser.UserName);
        var phone = PhoneNullable.Create(appUser.PhoneNumber);
        var firstName = FirstNameNullable.Create(appUser.FirstName);
        var lastName = LastNameNullable.Create(appUser.LastName);
        var password = Password.Create("Password123!");
        var confPassword = ConfirmPassword.Create("Password123!");
        var position = TeamPositionNullable.Create(1);
        Guid? subscriptionPlanId = null;
        var cancellationToken = CancellationToken.None;

        _teamManagerMock.Setup(m => m.AddTeamAsync(It.IsAny<Team>(), cancellationToken))
            .ReturnsAsync(team);
        _teamManagerMock.Setup(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), password.Value))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));
        _teamManagerMock.Setup(m => m.GetSubscriptionServiceAsync(team))
            .ReturnsAsync(GenResult<ITeamSubscriptionService>.Success(_subServiceMock.Object));
        _teamManagerMock.Setup(m => m.UpdateAsync(team))
            .ReturnsAsync(team);

        _subServiceMock.Setup(m => m.Team).Returns(team);
        _subServiceMock.Setup(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()))
            .ReturnsAsync(GenResult<TeamSubscription>.Success(SubscriptionDataFactory.Create()));

        // Act
        var result = await _regService.RegisterAsync(email, username, phone, firstName, lastName, password, confPassword, position, subscriptionPlanId, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Email.ShouldBe(email.Value);
        result.Value.UserName.ShouldBe(username.Value);
        _teamManagerMock.Verify(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), password.Value), Times.Once);
        _subServiceMock.Verify(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task RegisterAsync_ShouldReturnErrorWhen_regFails()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id);
        var email = EmailAddress.Create(appUser.Email!);
        var username = UsernameNullable.Create(appUser.UserName);
        var phone = PhoneNullable.Create(appUser.PhoneNumber);
        var firstName = FirstNameNullable.Create(appUser.FirstName);
        var lastName = LastNameNullable.Create(appUser.LastName);
        var password = Password.Create("Password123!");
        var confPassword = ConfirmPassword.Create("Password123!");
        var position = TeamPositionNullable.Create(1);
        Guid subscriptionPlanId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var regFailureMessage = "Something went Wrong during Reg!";

        _teamManagerMock.Setup(m => m.AddTeamAsync(It.IsAny<Team>(), cancellationToken))
            .ReturnsAsync(team);
        _teamManagerMock.Setup(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), password.Value))
            .ReturnsAsync(GenResult<AppUser>.Failure(regFailureMessage));
        _teamManagerMock.Setup(m => m.GetSubscriptionServiceAsync(team))
            .ReturnsAsync(GenResult<ITeamSubscriptionService>.Success(_subServiceMock.Object));
        _teamManagerMock.Setup(m => m.UpdateAsync(team))
            .ReturnsAsync(team);

        _subServiceMock.Setup(m => m.Team).Returns(team);
        _subServiceMock.Setup(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()))
            .ReturnsAsync(GenResult<TeamSubscription>.Success(SubscriptionDataFactory.Create()));

        // Act
        var result = await _regService.RegisterAsync(email, username, phone, firstName, lastName, password, confPassword, position, subscriptionPlanId, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Info.ShouldBe(regFailureMessage);
        _teamManagerMock.Verify(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), password.Value), Times.Once);
        _subServiceMock.Verify(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task RegisterAsync_ShouldReturnErrorWhen_AddSubscripitonFails()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id);
        var email = EmailAddress.Create(appUser.Email!);
        var username = UsernameNullable.Create(appUser.UserName);
        var phone = PhoneNullable.Create(appUser.PhoneNumber);
        var firstName = FirstNameNullable.Create(appUser.FirstName);
        var lastName = LastNameNullable.Create(appUser.LastName);
        var password = Password.Create("Password123!");
        var confPassword = ConfirmPassword.Create("Password123!");
        var position = TeamPositionNullable.Create(1);
        Guid subscriptionPlanId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var addSubFailureMessage = "Something went Wrong during AddSub!";

        _teamManagerMock.Setup(m => m.AddTeamAsync(It.IsAny<Team>(), cancellationToken))
            .ReturnsAsync(team);
        _teamManagerMock.Setup(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), password.Value))
            .ReturnsAsync(GenResult<AppUser>.Success(AppUserDataFactory.Create()));
        _teamManagerMock.Setup(m => m.GetSubscriptionServiceAsync(team))
            .ReturnsAsync(GenResult<ITeamSubscriptionService>.Success(_subServiceMock.Object));
        _teamManagerMock.Setup(m => m.UpdateAsync(team))
            .ReturnsAsync(team);

        _subServiceMock.Setup(m => m.Team).Returns(team);
        _subServiceMock.Setup(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()))
            .ReturnsAsync(GenResult<TeamSubscription>.Failure(addSubFailureMessage));

        // Act
        var result = await _regService.RegisterAsync(email, username, phone, firstName, lastName, password, confPassword, position, subscriptionPlanId, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Info.ShouldBe(addSubFailureMessage);
        _teamManagerMock.Verify(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), password.Value), Times.Once);
        _subServiceMock.Verify(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task RegisterAsync_ShouldReturnBadRequest_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id);
        var email = EmailAddress.Create(appUser.Email!);
        var username = UsernameNullable.Create(appUser.UserName);
        var phone = PhoneNullable.Create(appUser.PhoneNumber);
        var firstName = FirstNameNullable.Create(appUser.FirstName);
        var lastName = LastNameNullable.Create(appUser.LastName);
        var password = Password.Create("Password123!");
        var confPassword = ConfirmPassword.Create("!321drowssaP");
        var position = TeamPositionNullable.Create(1);
        Guid subscriptionPlanId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _teamManagerMock.Setup(m => m.AddTeamAsync(It.IsAny<Team>(), cancellationToken))
            .ReturnsAsync(team);
        _teamManagerMock.Setup(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), password.Value))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));
        _teamManagerMock.Setup(m => m.GetSubscriptionServiceAsync(team))
            .ReturnsAsync(GenResult<ITeamSubscriptionService>.Success(_subServiceMock.Object));
        _teamManagerMock.Setup(m => m.UpdateAsync(team))
            .ReturnsAsync(team);

        _subServiceMock.Setup(m => m.Team).Returns(team);
        _subServiceMock.Setup(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()))
            .ReturnsAsync(GenResult<TeamSubscription>.Success(SubscriptionDataFactory.Create()));

        // Act
        var result = await _regService.RegisterAsync(email, username, phone, firstName, lastName, password, confPassword, position, subscriptionPlanId, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.NON_MATCHING_PASSOWRDS);
    }

    //------------------------------------//

}//Cls
