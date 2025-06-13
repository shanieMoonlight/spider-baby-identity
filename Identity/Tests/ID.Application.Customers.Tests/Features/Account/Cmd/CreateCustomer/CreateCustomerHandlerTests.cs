//using Id.Application.Tests.Utility.Data;
//using ID.Application.Abstractions.Teams;
//using ID.Application.Dtos.User;
//using ID.Application.Features.Account.Cmd.Customer.AddCustomerMember.CustomerRegNoPwd;
//using ID.Application.Features.Account.Cmd.Customer.CustomerRegNoPwd;
//using ID.Application.Features.Common.Dtos.User;
//using ID.Domain.Entities.AppUsers;
//using ID.Domain.Entities.Teams;
//using ID.Tests.Data.Factories;
//using Moq;
//using MyResults;
//using Shouldly;

//namespace ID.Application.Tests.Features.Account.Cmd.CreateCustomer;
//public class CreateCustomerHandlerTests
//{
//    private readonly Mock<IIdTeamManager<AppUser>> _mockTeamMgr = new();

//    public CreateCustomerHandlerTests()
//    {

//        //Should always create team
//        _mockTeamMgr.Setup(um => um.AddTeamAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(TeamData.AnyTeam);
//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Should_CallRegisterMemberAsyncWithCustomerTeam()
//    {
//        //Arrange
//        var newTeam = TeamDataFactory.Create();
//        var newUser = AppUserDataFactory.Create(newTeam.Id);

//        _mockTeamMgr.Setup(um => um.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
//            .ReturnsAsync(GenResult<AppUser>.Success(newUser));

//        _mockTeamMgr.Setup(um => um.AddTeamAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(newTeam);

//        var cmd = new RegisterCustomerNoPwdCmd(new RegisterCustomerDto() { Email = "r@s.tud" });


//        RegisterCustomerCmdHandler handler = new(_mockTeamMgr.Object);

//        //Act
//        var result = await handler.Handle(cmd, default);

//        //Assert
//        result.Succeeded.ShouldBeTrue();
//        result.NotFound.ShouldBeFalse();
//        result.Value.ShouldNotBeNull();
//        result.Value.ShouldBeAssignableTo<AppUserDto>();
//        result.Value.Id.ShouldBe(newUser.Id);
//        result.Value.Email.ShouldBe(newUser.Email);
//        result.Value.UserName.ShouldBe(newUser.UserName);
//        result.Value.FirstName.ShouldBe(newUser.FirstName);
//        result.Value.LastName.ShouldBe(newUser.LastName);
//        result.Value.PhoneNumber.ShouldBe(newUser.PhoneNumber);
//        result.Value.TeamId.ShouldBe(newTeam.Id);

//        _mockTeamMgr.Verify(
//            x => x.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()),
//            Times.Once
//        );


//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Should_ReturnSuccessWhenCreateSucceeds()
//    {
//        //Arrange
//        var newTeam = TeamDataFactory.Create();
//        var newUser = AppUserDataFactory.Create(newTeam.Id);
//        var _mockTeamMgr = new Mock<IIdTeamManager<AppUser>>();
//        _mockTeamMgr.Setup(um => um.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
//            .ReturnsAsync(GenResult<AppUser>.Success(newUser));
//        _mockTeamMgr.Setup(um => um.AddTeamAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(newTeam);

//        var cmd = new RegisterCustomerNoPwdCmd(new RegisterCustomerDto() { Email = "r@s.tud" });


//        RegisterCustomerCmdHandler handler = new(_mockTeamMgr.Object);

//        //Act
//        var result = await handler.Handle(cmd, default);

//        //Assert
//        result.Succeeded.ShouldBeTrue();
//        result.NotFound.ShouldBeFalse();
//        result.Value.ShouldNotBeNull();
//        result.Value.Id.ShouldBe(newUser.Id);
//        result.Value.Email.ShouldBe(newUser.Email);
//        result.Value.UserName.ShouldBe(newUser.UserName);
//        result.Value.FirstName.ShouldBe(newUser.FirstName);
//        result.Value.LastName.ShouldBe(newUser.LastName);
//        result.Value.PhoneNumber.ShouldBe(newUser.PhoneNumber);
//        result.Value.TeamId.ShouldBe(newTeam.Id);
//        result.Value.ShouldBeAssignableTo<AppUserDto>();

//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Should_ReturnFailureWhenRegisterSucceeds()
//    {
//        //Arrange
//        var newTeam = TeamDataFactory.Create();
//        var newUser = AppUserDataFactory.Create(newTeam.Id);
//        var _mockTeamMgr = new Mock<IIdTeamManager<AppUser>>();
//        _mockTeamMgr.Setup(um => um.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
//            .ReturnsAsync(GenResult<AppUser>.Failure());
//        _mockTeamMgr.Setup(um => um.AddTeamAsync(It.IsAny<Team>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(newTeam);

//        var cmd = new RegisterCustomerNoPwdCmd(new RegisterCustomerDto() { Email = "r@s.tud" });

//        RegisterCustomerCmdHandler handler = new(_mockTeamMgr.Object);

//        //Act
//        var result = await handler.Handle(cmd, default);

//        //Assert
//        result.Succeeded.ShouldBeFalse();
//        result.Value.ShouldBeNull();

//    }

//    //------------------------------------//


//}//Cls
