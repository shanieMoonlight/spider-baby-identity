using ID.Domain.Claims.Utils;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Id.Application.Tests.Utility.ExtensionMethods;

public class ClaimsPrincipalExtensionsTests
{
    private ClaimsPrincipal CreatePrincipal(params Claim[] claims) => new(new ClaimsIdentity(claims, "mock"));

    [Fact]
    public void GetEmail_Returns_Email_From_JwtRegisteredClaimNames_Email()
    {
        var principal = CreatePrincipal(new Claim(JwtRegisteredClaimNames.Email, "jwt@email.com"));
        principal.GetEmail().ShouldBe("jwt@email.com");
    }

    [Fact]
    public void GetEmail_Returns_Email_From_MyIdClaimTypes_EMAIL()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.EMAIL, "myid@email.com"));
        principal.GetEmail().ShouldBe("myid@email.com");
    }

    [Fact]
    public void GetEmail_Returns_Email_From_ClaimTypes_Email()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Email, "dotnet@email.com"));
        principal.GetEmail().ShouldBe("dotnet@email.com");
    }

    [Fact]
    public void GetEmail_Returns_Null_If_No_Email_Claim()
    {
        var principal = CreatePrincipal();
        principal.GetEmail().ShouldBeNull();
    }

    //[Fact]
    //public void GetDeviceId_Returns_DeviceId_When_Valid_Subscription_Exists()
    //{
    //    var sub = new SubscriptionClaimData { Name = "planA", DeviceId = "device123" };
    //    var claim = new Claim(MyIdClaimTypes.TEAM_SUBSCRIPTIONS, JsonConvert.SerializeObject(sub));
    //    var principal = CreatePrincipal(claim);
    //    principal.GetDeviceId("planA").ShouldBe("device123");
    //}

    //[Fact]
    //public void GetDeviceId_Returns_Null_When_No_Matching_Subscription()
    //{
    //    var sub = new SubscriptionClaimData { Name = "planB", DeviceId = "device456" };
    //    var claim = new Claim(MyIdClaimTypes.TEAM_SUBSCRIPTIONS, JsonConvert.SerializeObject(sub));
    //    var principal = CreatePrincipal(claim);
    //    principal.GetDeviceId("planA").ShouldBeNull();
    //}

    [Fact]
    public void GetTeamId_Returns_Guid_When_Valid()
    {
        var guid = Guid.NewGuid();
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_ID, guid.ToString()));
        principal.GetTeamId().ShouldBe(guid);
    }

    [Fact]
    public void GetTeamId_Returns_Null_When_Invalid()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_ID, "not-a-guid"));
        principal.GetTeamId().ShouldBeNull();
    }

    [Fact]
    public void GetTeamType_Returns_Enum_When_Valid()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, "maintenance"));
        principal.GetTeamType().ShouldBe(TeamType.maintenance);
    }

    [Fact]
    public void GetTeamType_Returns_Customer_When_Invalid()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, "not-a-type"));
        principal.GetTeamType().ShouldBe(TeamType.customer);
    }

    [Fact]
    public void GetUserId_Returns_Guid_When_Valid()
    {
        var guid = Guid.NewGuid();
        var principal = CreatePrincipal(new Claim(JwtRegisteredClaimNames.Sub, guid.ToString()));
        principal.GetUserId().ShouldBe(guid);
    }

    [Fact]
    public void GetUserId_Returns_Null_When_Invalid()
    {
        var principal = CreatePrincipal(new Claim(JwtRegisteredClaimNames.Sub, "not-a-guid"));
        principal.GetUserId().ShouldBeNull();
    }

    [Fact]
    public void GetUsername_Returns_From_MyIdClaimTypes_NAME()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.NAME, "myname"));
        principal.GetUsername().ShouldBe("myname");
    }

    [Fact]
    public void GetUsername_Returns_From_JwtRegisteredClaimNames_Name()
    {
        var principal = CreatePrincipal(new Claim(JwtRegisteredClaimNames.Name, "jwtname"));
        principal.GetUsername().ShouldBe("jwtname");
    }

    [Fact]
    public void GetUsername_Returns_From_ClaimTypes_Name()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Name, "dotnetname"));
        principal.GetUsername().ShouldBe("dotnetname");
    }

    [Fact]
    public void GetUsername_Returns_From_ClaimTypes_NameIdentifier()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, "idname"));
        principal.GetUsername().ShouldBe("idname");
    }

    [Fact]
    public void HasClaim_Returns_True_If_Claim_Exists()
    {
        var claim = new Claim("type", "value");
        var principal = CreatePrincipal(claim);
        principal.HasClaim(claim).ShouldBeTrue();
    }

    [Fact]
    public void HasClaim_Returns_False_If_Claim_Does_Not_Exist()
    {
        var claim = new Claim("type", "value");
        var principal = CreatePrincipal();
        principal.HasClaim(claim).ShouldBeFalse();
    }

    [Fact]
    public void IsAuthenticated_Returns_True_If_Authenticated()
    {
        var identity = new ClaimsIdentity(new[] { new Claim("type", "value") }, "mock");
        var principal = new ClaimsPrincipal(identity);
        principal.IsAuthenticated().ShouldBeTrue();
    }

    [Fact]
    public void IsAuthenticated_Returns_False_If_Not_Authenticated()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        principal.IsAuthenticated().ShouldBeFalse();
    }

    [Fact]
    public void IsInCustomerTeam_Returns_True_If_Customer()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, MyTeamClaimValues.CUSTOMER_TEAM_NAME));
        principal.IsInCustomerTeam().ShouldBeTrue();
    }

    [Fact]
    public void IsInCustomerTeam_Returns_False_If_Not_Customer()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, "other"));
        principal.IsInCustomerTeam().ShouldBeFalse();
    }

    [Fact]
    public void IsInMntcTeam_Returns_True_If_Maintenance()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, MyTeamClaimValues.MAINTENANCE_TEAM_NAME));
        principal.IsInMntcTeam().ShouldBeTrue();
    }

    [Fact]
    public void IsInMntcTeam_Returns_False_If_Not_Maintenance()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, "other"));
        principal.IsInMntcTeam().ShouldBeFalse();
    }

    [Fact]
    public void IsInMntcTeamMinimum_Returns_True_If_Maintenance_Or_Super()
    {
        var mntc = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, MyTeamClaimValues.MAINTENANCE_TEAM_NAME));
        var super = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, MyTeamClaimValues.SUPER_TEAM_NAME));
        mntc.IsInMntcTeamMinimum().ShouldBeTrue();
        super.IsInMntcTeamMinimum().ShouldBeTrue();
    }

    [Fact]
    public void IsInMntcTeamMinimum_Returns_False_If_Neither()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, "other"));
        principal.IsInMntcTeamMinimum().ShouldBeFalse();
    }

    [Fact]
    public void IsInCustomerTeamMinimum_Returns_True_If_Customer_Or_MntcOrSuper()
    {
        var customer = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, MyTeamClaimValues.CUSTOMER_TEAM_NAME));
        var mntc = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, MyTeamClaimValues.MAINTENANCE_TEAM_NAME));
        var super = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, MyTeamClaimValues.SUPER_TEAM_NAME));
        customer.IsInCustomerTeamMinimum().ShouldBeTrue();
        mntc.IsInCustomerTeamMinimum().ShouldBeTrue();
        super.IsInCustomerTeamMinimum().ShouldBeTrue();
    }

    [Fact]
    public void IsInCustomerTeamMinimum_Returns_False_If_Neither()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, "other"));
        principal.IsInCustomerTeamMinimum().ShouldBeFalse();
    }

    [Fact]
    public void IsInRole_Returns_True_If_Any_Role_Claim_Matches()
    {
        var principal = CreatePrincipal(
            new Claim(MyIdClaimTypes.ROLE, "admin"),
            new Claim(ClaimTypes.Role, "user"),
            new Claim("role", "manager")
        );
        principal.IsInMyIdRole("admin").ShouldBeTrue();
        principal.IsInMyIdRole("user").ShouldBeTrue();
        principal.IsInMyIdRole("manager").ShouldBeTrue();
    }

    [Fact]
    public void IsInRole_Returns_False_If_No_Match()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.ROLE, "admin"));
        principal.IsInRole("user").ShouldBeFalse();
    }

    [Fact]
    public void IsInSuperTeam_Returns_True_If_Super()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, MyTeamClaimValues.SUPER_TEAM_NAME));
        principal.IsInSuperTeam().ShouldBeTrue();
    }

    [Fact]
    public void IsInSuperTeam_Returns_False_If_Not_Super()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_TYPE, "other"));
        principal.IsInSuperTeam().ShouldBeFalse();
    }

    [Fact]
    public void IsInTeam_Returns_True_If_TeamId_Matches()
    {
        var guid = Guid.NewGuid();
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_ID, guid.ToString()));
        principal.IsInTeam(guid).ShouldBeTrue();
    }

    [Fact]
    public void IsInTeam_Returns_False_If_TeamId_Does_Not_Match()
    {
        var guid = Guid.NewGuid();
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_ID, Guid.NewGuid().ToString()));
        principal.IsInTeam(guid).ShouldBeFalse();
    }

    [Fact]
    public void IsTeamLeader_Returns_True_If_Leader()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.ROLE, MyTeamClaimValues.LEADER));
        principal.IsTeamLeader().ShouldBeTrue();
    }

    [Fact]
    public void IsTeamLeader_Returns_False_If_Not_Leader()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.ROLE, "member"));
        principal.IsTeamLeader().ShouldBeFalse();
    }

    [Fact]
    public void TeamPosition_Returns_Int_When_Valid()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_POSITION, "5"));
        principal.TeamPosition().ShouldBe(5);
    }

    [Fact]
    public void TeamPosition_Returns_Minus1_When_Invalid()
    {
        var principal = CreatePrincipal(new Claim(MyIdClaimTypes.TEAM_POSITION, "not-an-int"));
        principal.TeamPosition().ShouldBe(-1);
    }

    [Fact]
    public void GetClaimValue_Returns_Value_If_Exists()
    {
        var principal = CreatePrincipal(new Claim("custom", "val"));
        principal.GetClaimValue("custom").ShouldBe("val");
    }

    [Fact]
    public void GetClaimValue_Returns_Null_If_Not_Exists()
    {
        var principal = CreatePrincipal();
        principal.GetClaimValue("custom").ShouldBeNull();
    }

    [Fact]
    public void GetClaimValues_Returns_Values_If_Exists()
    {
        var principal = CreatePrincipal(
            new Claim("custom", "val1"),
            new Claim("custom", "val2")
        );
        var values = principal.GetClaimValues("custom");
        values.ShouldNotBeNull();
        values.ShouldContain("val1");
        values.ShouldContain("val2");
    }

    [Fact]
    public void GetClaimValues_Returns_Null_If_Not_Exists()
    {
        var principal = CreatePrincipal();
        principal.GetClaimValues("custom").ShouldBeEmpty();
    }
}
            