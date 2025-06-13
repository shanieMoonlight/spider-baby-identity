using ID.Application.AppAbs.ApplicationServices.Principal;
using ID.Application.Mediatr.Behaviours;
using ID.Application.Mediatr.Cqrslmps;
using MediatR;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Pipeline;

public class IdPrincipalPipelineBehaviorTests
{
    private readonly Mock<IIdPrincipalInfo> _mockUserInfo;
    private readonly Mock<RequestHandlerDelegate<BasicResult>> _mockNext;
    private readonly IdPrincipalPipelineBehavior<TestUserAwareCommand, BasicResult> _behavior;

    //- - - - - - - - - - - - - - - - - - //

    public IdPrincipalPipelineBehaviorTests()
    {
        _mockUserInfo = new Mock<IIdPrincipalInfo>();
        _mockNext = new Mock<RequestHandlerDelegate<BasicResult>>();
        _behavior = new IdPrincipalPipelineBehavior<TestUserAwareCommand, BasicResult>(_mockUserInfo.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldAttachUserInfoToRequest()
    {
        // Arrange
        var request = new TestUserAwareCommand
        {
            PrincipalTeamId = Guid.NewGuid()
        };


        var response = BasicResult.Success();
        _mockNext.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(response);

        _mockUserInfo.Setup(x => x.IsAuthenticated()).Returns(true);
        _mockUserInfo.Setup(x => x.UserId()).Returns(Guid.NewGuid());
        _mockUserInfo.Setup(x => x.TeamId()).Returns(Guid.NewGuid());
        _mockUserInfo.Setup(x => x.TeamPositionValue()).Returns(1);
        _mockUserInfo.Setup(x => x.Email()).Returns("test@example.com");
        _mockUserInfo.Setup(x => x.Username()).Returns("testuser");
        _mockUserInfo.Setup(x => x.IsSpr()).Returns(true);
        _mockUserInfo.Setup(x => x.IsSprMinimum()).Returns(true);
        _mockUserInfo.Setup(x => x.IsMntc()).Returns(true);
        _mockUserInfo.Setup(x => x.IsMntcMinimum()).Returns(true);
        _mockUserInfo.Setup(x => x.IsCustomer()).Returns(true);
        _mockUserInfo.Setup(x => x.IsCustomerMinimum()).Returns(true);
        _mockUserInfo.Setup(x => x.GetPrincipal()).Returns(new System.Security.Claims.ClaimsPrincipal());

        // Act
        var result = await _behavior.Handle(request, _mockNext.Object, CancellationToken.None);

        // Assert
        request.IsAuthenticated.ShouldBe(true);
        request.PrincipalUserId.ShouldBe(_mockUserInfo.Object.UserId());
        request.PrincipalTeamId.ShouldBe(_mockUserInfo.Object.TeamId());
        request.PrincipalTeamPosition.ShouldBe(_mockUserInfo.Object.TeamPositionValue());
        request.PrincipalEmail.ShouldBe(_mockUserInfo.Object.Email());
        request.PrincipalUsername.ShouldBe(_mockUserInfo.Object.Username());
        request.IsSuper.ShouldBe(_mockUserInfo.Object.IsSpr());
        request.IsSuperMinimum.ShouldBe(_mockUserInfo.Object.IsSprMinimum());
        request.IsMntc.ShouldBe(_mockUserInfo.Object.IsMntc());
        request.IsMntcMinimum.ShouldBe(_mockUserInfo.Object.IsMntcMinimum());
        request.IsCustomer.ShouldBe(_mockUserInfo.Object.IsCustomer());
        request.IsCustomerMinimum.ShouldBe(_mockUserInfo.Object.IsCustomerMinimum());
        request.Principal.ShouldBe(_mockUserInfo.Object.GetPrincipal());
        result.ShouldBe(response);
    }
}





//=============================================================================//

public record TestUserAwareCommand : APrincipalInfoRequest, IRequest<BasicResult>
{
}


//=============================================================================//


