using ID.Application.JWT.Subscriptions;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Claims;
using ID.Domain.Claims.Utils;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace ID.Tests.Data.Filters;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class ContextProvider
{
    public static HttpContext GetHttpContext(IEnumerable<Claim> claims, bool isAuthenticated = true)
    {
        var identity = isAuthenticated
            ? new ClaimsIdentity(claims, "AthenticationType_IamAmAuthenticated")
            : new ClaimsIdentity(claims);

        return new DefaultHttpContext
        {
            User = new ClaimsPrincipal([identity])
        };
    }

    //- - - - - - - - - - - - - - - //

    public static HttpContext GetHttpContext(bool isDevMode, IEnumerable<Claim> claims, bool isAuthenticated = true)
    {
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        if (isDevMode)
            mockEnvironment.SetupGet(e => e.EnvironmentName).Returns("Development");
        else
            mockEnvironment.SetupGet(e => e.EnvironmentName).Returns("Production");
        var services = new ServiceCollection();
        services.AddSingleton(mockEnvironment.Object);
        var serviceProvider = services.BuildServiceProvider();

        var httpContext = GetHttpContext(claims, isAuthenticated);
        httpContext.RequestServices = serviceProvider;
        return httpContext;
    }


    //------------------------------//


    public static ResourceExecutingContext GetResourceExecutingContext(IEnumerable<Claim> claims, bool isAuthenticated = true)
    {
        var httpContext = GetHttpContext(claims, isAuthenticated);
        var routeData = new RouteData();
        var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());
        return new ResourceExecutingContext(actionContext, [], []);
    }

    //- - - - - - - - - - - - - - - //

    public static ResourceExecutingContext GetResourceExecutingContext(HttpContext httpContext)
    {
        var routeData = new RouteData();
        var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());
        return new ResourceExecutingContext(actionContext, [], []);
    }

    //------------------------------//

    public static ActionContext GetActionContext(IEnumerable<Claim> claims, bool isAuthenticated = true)
    {
        var httpContext = GetHttpContext(claims, isAuthenticated);
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        return new ActionContext(httpContext, routeData, actionDescriptor);
    }

    //- - - - - - - - - - - - - - - //

    public static ActionContext GetActionContext(HttpContext httpContext)
    {
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        return new ActionContext(httpContext, routeData, actionDescriptor);
    }

    //------------------------------//

    public static ActionExecutingContext GetActionExecutingContext(IEnumerable<Claim> claims, bool isAuthenticated = true) =>
        new(
            GetActionContext(claims, isAuthenticated),
            [],
            new Dictionary<string, object?>(),
            new object()
        );

    //- - - - - - - - - - - - - - - //

    public static ActionExecutingContext GetActionExecutingContext(HttpContext httpContext) =>
        new(
            GetActionContext(httpContext),
            [],
            new Dictionary<string, object?>(),
            new object()
        );

    //------------------------------//
    public static ActionContext GetActionContext()
    {
        var identity = new ClaimsIdentity([], "AthenticationType_IamAmAuthenticated");
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal([identity])
        };
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        return new ActionContext(httpContext, routeData, actionDescriptor);
    }

    //-------------------------------------//

    public static ResourceExecutingContext GetActionContextWithDeviceIdClaim(IEnumerable<SubscriptionClaimData> subClaimData)
    {
        var httpContext = new DefaultHttpContext();
        var claims = subClaimData.Select(d => d.ToClaim())
            .ToList();

        var identity = new ClaimsIdentity(claims, "AthenticationType_IamAmAuthenticated");
        var user = new ClaimsPrincipal([identity]);
        httpContext.User = user;

        var routeData = new RouteData();
        var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());
        return new ResourceExecutingContext(actionContext, [], []);
    }

    //-------------------------------------//

    public static ActionContext GetActionContextWithTwoFactorVerified()
    {
        var claims = new List<Claim>
        {
            TwoFactorClaims.TwoFactorVerified
        };
        var identity = new ClaimsIdentity(claims, "IamAuthenticatedAuthenticationType");
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal([identity])
        };

        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        return new ActionContext(httpContext, routeData, actionDescriptor);
    }

    //-------------------------------------//

    public static ActionContext GetActionContextWithInitializedTeam()
    {
        var httpContext = new DefaultHttpContext();

        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(
            teamId: teamId,
            id: userId);
        var team = TeamDataFactory.Create(
            leaderId: userId,
            leader: user);

        var claims = new List<Claim>
        {
            TwoFactorClaims.TwoFactorVerified,
        };

        var identity = new ClaimsIdentity(claims);
        var userPrincipal = new ClaimsPrincipal([identity]);

        httpContext.User = userPrincipal;

        var svc = new Mock<IServiceProvider>();
        var teamMgr = new Mock<IIdentityTeamManager<AppUser>>();
        teamMgr.Setup(repo => repo.GetSuperTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(team);
        svc.Setup(s => s.GetService(typeof(IIdentityTeamManager<AppUser>))).Returns(teamMgr.Object);
        httpContext.RequestServices = svc.Object;
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        return new ActionContext(httpContext, routeData, actionDescriptor);
    }

    //-------------------------------------//

    public static ActionContext GetActionContextWithUninitializedTeam()
    {
        var httpContext = new DefaultHttpContext();
        var svc = new Mock<IServiceProvider>();
        var teamRepo = new Mock<IIdentityTeamManager<AppUser>>();
        teamRepo.Setup(repo => repo.GetSuperTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync((Team)null);
        svc.Setup(s => s.GetService(typeof(IIdentityTeamManager<AppUser>))).Returns(teamRepo.Object);
        httpContext.RequestServices = svc.Object;
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        return new ActionContext(httpContext, routeData, actionDescriptor);
    }

    //-------------------------------------//

    public static ActionContext GetActionContextWithSuperLeader()
    {
        var httpContext = new DefaultHttpContext();

   
        var claims = new List<Claim>
        {
            TwoFactorClaims.TwoFactorVerified,
            new(MyIdClaimTypes.TEAM_TYPE,  MyTeamClaimValues.SUPER_TEAM_NAME),
            IdTeamClaims.LEADER
        };

        var identity = new ClaimsIdentity(claims);
        var userPrincipal = new ClaimsPrincipal([identity]);

        httpContext.User = userPrincipal;
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        return new ActionContext(httpContext, routeData, actionDescriptor);
    }

    //-------------------------------------//

    public static ResourceExecutingContext GetResourceExecutingContext()
    {
        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());
        return new ResourceExecutingContext(actionContext, [], []);
    }

    //-------------------------------------//

    public static ActionContext GetActionContextWithTeamTypeAndPosition(
        string teamClaimValue,
        bool isLeader,
        int position,
        bool tfVerified)
    {
        var httpContext = new DefaultHttpContext();

        var claims = new List<Claim>
        {
            IdTeamClaim.GenerateTeamPositionClaim(position),
            new(MyIdClaimTypes.TEAM_TYPE, teamClaimValue)
        };

        if (tfVerified)
            claims.Add(TwoFactorClaims.TwoFactorVerified);

        if (isLeader)
            claims.Add(IdTeamClaims.LEADER);

        var identity = new ClaimsIdentity(claims, "IamAuthenticatedAuthenticationType");
        var userPrincipal = new ClaimsPrincipal([identity]);

        httpContext.User = userPrincipal;
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        return new ActionContext(httpContext, routeData, actionDescriptor);
    }


}//Cls
