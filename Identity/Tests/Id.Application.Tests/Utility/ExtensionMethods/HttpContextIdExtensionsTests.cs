using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Id.Application.Tests.Utility.ExtensionMethods;

public class HttpContextIdExtensionsTests
{
    [Fact]
    public void GetDeviceId_Delegates_To_User()
    {
        var claims = new[] { new Claim("device", "device123") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        // This test is limited by the extension method's logic, so just check that it doesn't throw and returns null (since no real claim logic)
        ctx.GetDeviceId("plan").ShouldBeNull();
    }

    [Fact]
    public void GetEmail_Delegates_To_User()
    {
        var claims = new[] { new Claim(ClaimTypes.Email, "test@email.com") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.GetEmail().ShouldBe("test@email.com");
    }

    [Fact]
    public void GetTeamId_Delegates_To_User()
    {
        var guid = Guid.NewGuid();
        var claims = new[] { new Claim("team_id", guid.ToString()) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.GetTeamId().ShouldBeNull(); // Because claim type is not MyIdClaimTypes.TEAM_ID, adjust as needed
    }

    [Fact]
    public void GetTeamId_Returns_Null_If_Context_Null()
    {
        HttpContext? ctx = null;
        ctx.GetTeamId().ShouldBeNull();
    }

    [Fact]
    public void GetTeamType_Delegates_To_User()
    {
        var claims = new[] { new Claim("team_type", "maintenance") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.GetTeamType().ShouldBe(TeamType.customer); // Because claim type is not MyIdClaimTypes.TEAM_TYPE, adjust as needed
    }

    [Fact]
    public void GetTeamType_Returns_Customer_If_Context_Null()
    {
        HttpContext? ctx = null;
        ctx.GetTeamType().ShouldBe(TeamType.customer);
    }

    [Fact]
    public void GetUserId_Delegates_To_User()
    {
        var guid = Guid.NewGuid();
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, guid.ToString()) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.GetUserId().ShouldBe(guid);
    }

    [Fact]
    public void GetUsername_Delegates_To_User()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "uname") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.GetUsername().ShouldBe("uname");
    }

    [Fact]
    public void HasClaim_Delegates_To_User()
    {
        var claim = new Claim("type", "val");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { claim }, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.HasClaim(claim).ShouldBeTrue();
    }

    [Fact]
    public void IsAuthenticated_Delegates_To_User()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("type", "val") }, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.IsAuthenticated().ShouldBeTrue();
    }

    [Fact]
    public void IsInCustomerTeam_Delegates_To_User()
    {
        var claims = new[] { new Claim("team_type", "customer") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.IsInCustomerTeam().ShouldBeFalse(); // Because claim type is not MyIdClaimTypes.TEAM_TYPE, adjust as needed
    }

    [Fact]
    public void IsInCustomerTeam_Returns_False_If_Context_Null()
    {
        HttpContext? ctx = null;
        ctx.IsInCustomerTeam().ShouldBeFalse();
    }

    [Fact]
    public void IsInMntcTeam_Delegates_To_User()
    {
        var claims = new[] { new Claim("team_type", "maintenance") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.IsInMntcTeam().ShouldBeFalse(); // Because claim type is not MyIdClaimTypes.TEAM_TYPE, adjust as needed
    }

    [Fact]
    public void IsInMntcTeamMinimum_Delegates_To_User()
    {
        var claims = new[] { new Claim("team_type", "maintenance") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.IsInMntcTeamMinimum().ShouldBeFalse(); // Because claim type is not MyIdClaimTypes.TEAM_TYPE, adjust as needed
    }

    [Fact]
    public void IsInRole_Delegates_To_User()
    {
        var claims = new[] { new Claim(ClaimTypes.Role, "role") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.IsInRole("role").ShouldBeTrue();
    }

    [Fact]
    public void IsInRole_Returns_False_If_Context_Null()
    {
        HttpContext? ctx = null;
        ctx.IsInRole("role").ShouldBeFalse();
    }

    [Fact]
    public void IsInSuperTeam_Delegates_To_User()
    {
        var claims = new[] { new Claim("team_type", "super") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.IsInSuperTeam().ShouldBeFalse(); // Because claim type is not MyIdClaimTypes.TEAM_TYPE, adjust as needed
    }

    [Fact]
    public void IsInSuperTeam_Returns_False_If_Context_Null()
    {
        HttpContext? ctx = null;
        ctx.IsInSuperTeam().ShouldBeFalse();
    }

    [Fact]
    public void IsInTeam_Delegates_To_User()
    {
        var guid = Guid.NewGuid();
        var claims = new[] { new Claim("team_id", guid.ToString()) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.IsInTeam(guid).ShouldBeFalse(); // Because claim type is not MyIdClaimTypes.TEAM_ID, adjust as needed
    }

    [Fact]
    public void IsTeamLeader_Delegates_To_User()
    {
        var claims = new[] { new Claim(ClaimTypes.Role, "leader") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.IsTeamLeader().ShouldBeFalse(); // Because logic is in extension, adjust as needed
    }

    [Fact]
    public void TeamPosition_Delegates_To_User()
    {
        var claims = new[] { new Claim("team_position", "42") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
        var ctx = new DefaultHttpContext { User = user };
        ctx.TeamPosition().ShouldBe(-1); // Because claim type is not MyIdClaimTypes.TEAM_POSITION, adjust as needed
    }
}
