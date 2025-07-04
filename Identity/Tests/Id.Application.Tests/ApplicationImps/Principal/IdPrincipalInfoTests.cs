using ClArch.ValueObjects;
using ID.Tests.Data.Factories;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ID.Application.AppImps.User;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Devices;
using ID.Domain.Claims;
using ID.Application.JWT.Subscriptions;

namespace ID.Application.Tests.ApplicationImps.Principal;

public class IdPrincipalInfoTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<HttpContext> _httpContextMock;
    //private readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;
    //private readonly IdPrincipalInfo _idPrincipalInfo;

    public IdPrincipalInfoTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextMock = new Mock<HttpContext>();
        //_claimsPrincipalMock = new Mock<ClaimsPrincipal>();

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);
        //_httpContextMock.Setup(x => x.User).Returns(_claimsPrincipalMock.Object);

    }

    //------------------------------//
    [Fact]
    public void IsAuthenticated_ShouldReturnTrue_WhenUserIsAuthenticated()
    {
        //_claimsPrincipalMock.Setup(x => x.Identity.IsAuthenticated).Returns(true);

        ClaimsIdentity identity = ClaimsIdentityDataFactory.Create([], true);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsAuthenticated();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void IsAuthenticated_ShouldReturnFalse_WhenUserIsNotAuthenticated()
    {

        ClaimsIdentity identity = ClaimsIdentityDataFactory.Create([], false);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsAuthenticated();
        result.ShouldBeFalse();
    }

    //------------------------------//

    [Fact]
    public void IsCustomer_ShouldReturnTrue_WhenUserIsInCustomerTeam()
    {
        ClaimsIdentity identity = new([IdTeamClaims.CUSTOMER_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsCustomer();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void IsCustomer_ShouldReturnFalse_WhenUserIs_NOT_InCustomerTeam()
    {
        ClaimsIdentity identity = new([IdTeamClaims.MAINTENANCE_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsCustomer();
        result.ShouldBeFalse();
    }

    //------------------------------//

    [Fact]
    public void IsMntc_ShouldReturnTrue_WhenUserIsInMntcTeam()
    {
        ClaimsIdentity identity = new([IdTeamClaims.MAINTENANCE_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsMntc();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void IsMntc_ShouldReturnFalse_WhenUserIs_NOT_InCMntcTeam()
    {
        ClaimsIdentity identity = new([IdTeamClaims.CUSTOMER_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsMntc();
        result.ShouldBeFalse();
    }

    //------------------------------//

    [Fact]
    public void IsSpr_ShouldReturnTrue_WhenUserIsInSPRTeam()
    {
        ClaimsIdentity identity = new([IdTeamClaims.MAINTENANCE_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsMntc();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void IsSpr_ShouldReturnFalse_WhenUserIs_NOT_InSprTeam()
    {
        ClaimsIdentity identity = new([IdTeamClaims.CUSTOMER_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsMntc();
        result.ShouldBeFalse();
    }

    //------------------------------//

    [Fact]
    public void IsCustomerMinimum_ShouldReturnTrue_WhenUserIsCustomer()
    {
        ClaimsIdentity identity = new([IdTeamClaims.CUSTOMER_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsCustomerMinimum();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void IsCustomerMinimum_ShouldReturnTrue_WhenUserIsMntc()
    {
        ClaimsIdentity identity = new([IdTeamClaims.MAINTENANCE_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsCustomerMinimum();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void IsCustomerMinimum_ShouldReturnTrue_WhenUserIsSpr()
    {
        ClaimsIdentity identity = new([IdTeamClaims.SUPER_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsCustomerMinimum();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void IsCustomerMinimum_ShouldReturnFalse_WhenNOTAnyTeam()
    {
        ClaimsIdentity identity = new([]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsCustomerMinimum();
        result.ShouldBeFalse();
    }

    //------------------------------//

    [Fact]
    public void IsMntcMinimum_ShouldReturnTrue_WhenUserIsMntc()
    {
        ClaimsIdentity identity = new([IdTeamClaims.MAINTENANCE_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsMntcMinimum();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void IsMntcMinimum_ShouldReturnTrue_WhenUserIsSpr()
    {
        ClaimsIdentity identity = new([IdTeamClaims.SUPER_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsMntcMinimum();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void IsMntcMinimum_ShouldReturnFalse_WhenIsCustomer()
    {
        ClaimsIdentity identity = new([IdTeamClaims.CUSTOMER_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsMntcMinimum();
        result.ShouldBeFalse();
    }

    //------------------------------//

    [Fact]
    public void IsSprMinimum_ShouldReturnTrue_WhenUserIsSpr()
    {
        ClaimsIdentity identity = new([IdTeamClaims.SUPER_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsSprMinimum();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void IsSprMinimum_ShouldReturnFalse_WhenIsCustomer()
    {
        ClaimsIdentity identity = new([IdTeamClaims.CUSTOMER_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsSprMinimum();
        result.ShouldBeFalse();
    }

    //------------------------------//

    [Fact]
    public void IsSprMinimum_ShouldReturnFalse_WhenIsMntc()
    {
        ClaimsIdentity identity = new([IdTeamClaims.MAINTENANCE_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsSprMinimum();
        result.ShouldBeFalse();
    }

    //------------------------------//

    [Fact]
    public void TeamId_ShouldReturnTeamId()
    {
        var teamId = Guid.NewGuid();
        ClaimsIdentity identity = new([IdTeamClaims.TEAM_ID(teamId)]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.TeamId();
        result.ShouldBe(teamId);
    }

    //------------------------------//

    [Fact]
    public void UserId_ShouldReturnUserId()
    {
        var userId = Guid.NewGuid();
        ClaimsIdentity identity = new([new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.UserId();
        result.ShouldBe(userId);
    }

    //------------------------------//

    [Fact]
    public void TeamPositionValue_ShouldReturnTeamPositionValue()
    {
        var teamPosition = 5;

        ClaimsIdentity identity = new([IdTeamClaims.TEAM_POSITION(teamPosition)]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.TeamPositionValue();
        result.ShouldBe(teamPosition);
    }

    //------------------------------//

    [Fact]
    public void IsLeader_ShouldReturnTrue_WhenUserIsTeamLeader()
    {

        ClaimsIdentity identity = new([IdTeamClaims.LEADER]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.IsLeader();
        result.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public void TeamType_ShouldReturnTeamType()
    {
        ClaimsIdentity identity = new([IdTeamClaims.CUSTOMER_TEAM]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.TeamType();
        result.ShouldBe(TeamType.customer);
    }

    //------------------------------//

    [Fact]
    public void Email_ShouldReturnEmail()
    {
        var email = "test@example.com";
        ClaimsIdentity identity = new([new Claim(ClaimTypes.Email, email)]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.Email();
        result.ShouldBe(email);
    }

    //------------------------------//

    [Fact]
    public void Username_ShouldReturnUsername()
    {
        var username = "testuser";
        ClaimsIdentity identity = new([new Claim(ClaimTypes.Name, username)]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);

        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);
        var result = idPrincipalInfo.Username();
        result.ShouldBe(username);
    }

    //------------------------------//

    [Fact]
    public void DeviceId_ShouldReturnDeviceId()
    {

        var subName = "mysub";
        var subId = Guid.NewGuid();
        var subPlan = SubscriptionPlanDataFactory.Create();
        var dvcUniqueId = "my_unique_id";
        var dvcName = "testDevice";
        var device = DeviceDataFactory.Create(subscriptionId: subId, uniqueId: dvcUniqueId, name: dvcName);
        var sub = SubscriptionDataFactory.Create(id: subId, plan: subPlan, name: subName, devices: [device]);
        sub.AddDevice(Name.Create(dvcName), DescriptionNullable.Create(null), UniqueId.Create(dvcUniqueId));

        ClaimsIdentity identity = new([sub.ToClaim(device.UniqueId)]);
        var principal = new ClaimsPrincipal(identity);
        _httpContextMock.Setup(x => x.User).Returns(principal);
        var idPrincipalInfo = new IdPrincipalInfo(_httpContextAccessorMock.Object);

        var result = idPrincipalInfo.DeviceId(subPlan.Name);
        result.ShouldBe(device.Id.ToString());
    }

}//Cls
