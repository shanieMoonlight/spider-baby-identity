using ID.Application.Customers.ApplicationImps;
using ID.Application.Customers.Dtos.User;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.AppServices.Abs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Customers.Tests.ApplicationImps;

public class CustomerRegistrationServiceTests_Dto
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly IdCustomerRegistrationService _regService;
    private readonly Mock<ITeamSubscriptionService> _subServiceMock;
    private readonly Mock<ITeamBuilderService> _teamBuilderMock;

    public CustomerRegistrationServiceTests_Dto()
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
        Guid subscriptionPlanId = Guid.NewGuid();
        var dto = new RegisterCustomerDto
        {
            TeamPosition = appUser.TeamPosition,
            Email = appUser.Email!,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            Username = appUser.UserName,
            PhoneNumber = appUser.PhoneNumber,
            Password = "password",
            ConfirmPassword = "password",
            SubscriptionPlanId = subscriptionPlanId
        };

        var cancellationToken = CancellationToken.None;

        _teamManagerMock.Setup(m => m.AddTeamAsync(It.IsAny<Team>(), cancellationToken))
            .ReturnsAsync(team);
        _teamManagerMock.Setup(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));
        _teamManagerMock.Setup(m => m.GetSubscriptionServiceAsync(team))
            .ReturnsAsync(GenResult<ITeamSubscriptionService>.Success(_subServiceMock.Object));
        _teamManagerMock.Setup(m => m.UpdateAsync(team))
            .ReturnsAsync(team);

        _subServiceMock.Setup(m => m.Team).Returns(team);
        _subServiceMock.Setup(m => m.AddSubscriptionAsync(It.IsAny<Guid>(), It.IsAny<Discount>()))
            .ReturnsAsync(GenResult<TeamSubscription>.Success(SubscriptionDataFactory.Create()));



        // Act
        var result = await _regService.RegisterAsync(dto, cancellationToken);



        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        _teamManagerMock.Verify(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), It.IsAny<string>()), Times.Once);
        _subServiceMock.Verify(m => m.AddSubscriptionAsync(It.IsAny<Guid>(), It.IsAny<Discount>()), Times.Once);
        _teamManagerMock.Verify(m => m.UpdateAsync(team), Times.AtLeastOnce);
    }

    //------------------------------------//

    [Fact]
    public async Task RegisterAsync_ShouldRegisterCustomerSuccessfully_WhenNoSupsriptiopnPlanId()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id);
        Guid subscriptionPlanId = Guid.NewGuid();
        var dto = new RegisterCustomerDto
        {
            TeamPosition = appUser.TeamPosition,
            Email = appUser.Email!,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            Username = appUser.UserName,
            PhoneNumber = appUser.PhoneNumber,
            Password = "password",
            ConfirmPassword = "password",
            SubscriptionPlanId = null
        };
        var cancellationToken = CancellationToken.None;

        _teamManagerMock.Setup(m => m.AddTeamAsync(It.IsAny<Team>(), cancellationToken))
            .ReturnsAsync(team);
        _teamManagerMock.Setup(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), dto.Password))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));
        _teamManagerMock.Setup(m => m.GetSubscriptionServiceAsync(team))
            .ReturnsAsync(GenResult<ITeamSubscriptionService>.Success(_subServiceMock.Object));
        _teamManagerMock.Setup(m => m.UpdateAsync(team))
            .ReturnsAsync(team);

        _subServiceMock.Setup(m => m.Team).Returns(team);
        _subServiceMock.Setup(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()))
            .ReturnsAsync(GenResult<TeamSubscription>.Success(SubscriptionDataFactory.Create()));

        // Act
        var result = await _regService.RegisterAsync(dto, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Email.ShouldBe(dto.Email);
        result.Value.UserName.ShouldBe(dto.Username);
        _teamManagerMock.Verify(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), dto.Password), Times.Once);
        _subServiceMock.Verify(m => m.AddSubscriptionAsync(subscriptionPlanId, It.IsAny<Discount>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task RegisterAsync_ShouldReturnErrorWhen_regFails()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id);
        Guid subscriptionPlanId = Guid.NewGuid();
        var dto = new RegisterCustomerDto
        {
            TeamPosition = appUser.TeamPosition,
            Email = appUser.Email!,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            Username = appUser.UserName,
            PhoneNumber = appUser.PhoneNumber,
            Password = "password",
            ConfirmPassword = "password",
            SubscriptionPlanId = subscriptionPlanId
        };
        var cancellationToken = CancellationToken.None;
        var regFailureMessage = "Something went Wrong during Reg!";

        _teamManagerMock.Setup(m => m.AddTeamAsync(It.IsAny<Team>(), cancellationToken))
            .ReturnsAsync(team);
        _teamManagerMock.Setup(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(GenResult<AppUser>.Failure(regFailureMessage));
        _teamManagerMock.Setup(m => m.GetSubscriptionServiceAsync(team))
            .ReturnsAsync(GenResult<ITeamSubscriptionService>.Success(_subServiceMock.Object));
        _teamManagerMock.Setup(m => m.UpdateAsync(team))
            .ReturnsAsync(team);

        _subServiceMock.Setup(m => m.Team).Returns(team);
        _subServiceMock.Setup(m => m.AddSubscriptionAsync(It.IsAny<Guid>(), It.IsAny<Discount>()))
            .ReturnsAsync(GenResult<TeamSubscription>.Success(SubscriptionDataFactory.Create()));

        // Act
        var result = await _regService.RegisterAsync(dto, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Info.ShouldBe(regFailureMessage);
        _teamManagerMock.Verify(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), It.IsAny<string>()), Times.Once);
        _subServiceMock.Verify(m => m.AddSubscriptionAsync(It.IsAny<Guid>(), It.IsAny<Discount>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task RegisterAsync_ShouldReturnErrorWhen_AddSubscriptionFails()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id);
        Guid subscriptionPlanId = Guid.NewGuid();
        var dto = new RegisterCustomerDto
        {
            TeamPosition = appUser.TeamPosition,
            Email = appUser.Email!,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            Username = appUser.UserName,
            PhoneNumber = appUser.PhoneNumber,
            Password = "password",
            ConfirmPassword = "password",
            SubscriptionPlanId = subscriptionPlanId
        };
        var cancellationToken = CancellationToken.None;
        var addSubFailureMessage = "Something went Wrong during AddSub!";

        _teamManagerMock.Setup(m => m.AddTeamAsync(It.IsAny<Team>(), cancellationToken))
            .ReturnsAsync(team);
        _teamManagerMock.Setup(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));
        _teamManagerMock.Setup(m => m.GetSubscriptionServiceAsync(team))
            .ReturnsAsync(GenResult<ITeamSubscriptionService>.Success(_subServiceMock.Object));
        _teamManagerMock.Setup(m => m.UpdateAsync(team))
            .ReturnsAsync(team);

        _subServiceMock.Setup(m => m.Team).Returns(team);
        _subServiceMock.Setup(m => m.AddSubscriptionAsync(It.IsAny<Guid>(), It.IsAny<Discount>()))
            .ReturnsAsync(GenResult<TeamSubscription>.Failure(addSubFailureMessage));

        // Act
        var result = await _regService.RegisterAsync(dto, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(addSubFailureMessage);
        _teamManagerMock.Verify(m => m.RegisterMemberWithPasswordAsync(team, It.IsAny<AppUser>(), It.IsAny<string>()), Times.Once);
        _subServiceMock.Verify(m => m.AddSubscriptionAsync(It.IsAny<Guid>(), It.IsAny<Discount>()), Times.Once);
    }

    //------------------------------------//

}//Cls
